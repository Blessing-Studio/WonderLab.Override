using System;
using Avalonia;
using System.Linq;
using System.Numerics;
using Avalonia.Controls;
using WonderLab.Extensions;
using SixLabors.ImageSharp;
using Avalonia.Media.Imaging;
using System.Threading.Tasks;
using System.Collections.Frozen;
using Avalonia.Controls.Primitives;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Color = SixLabors.ImageSharp.Color;
using SixLabors.ImageSharp.Drawing.Processing;

using Image = Avalonia.Controls.Image;
using Rectangle = SixLabors.ImageSharp.Rectangle;
using LinearGradientBrush = SixLabors.ImageSharp.Drawing.Processing.LinearGradientBrush;
using Avalonia.Threading;

namespace WonderLab.Views.Controls;

public sealed class ImageBox : TemplatedControl {
    private Image _image;
    private ProgressRing _progressRing;

    private readonly Color DarkOverlayColor = Color.ParseHex("#1C1C1C");
    private readonly Color LightOverlayColor = Color.ParseHex("#F6F6F6");

    public static readonly StyledProperty<string> SourceProperty =
        AvaloniaProperty.Register<ImageBox, string>(nameof(Source));

    public static readonly StyledProperty<bool> IsEnableBlurProperty =
        AvaloniaProperty.Register<ImageBox, bool>(nameof(IsEnableBlur));

    public string Source {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public bool IsEnableBlur {
        get => GetValue(IsEnableBlurProperty);
        set => SetValue(IsEnableBlurProperty, value);
    }

    private void ApplyOpacityMask(Image<Rgba32> image, Image<Rgba32> mask) {
        Image<Rgba32> image2 = image;
        mask.Mutate(delegate (IImageProcessingContext x) {
            x.Resize(image2.Width, image2.Height);
        });
        for (int i = 0; i < image2.Height; i++) {
            for (int j = 0; j < image2.Width; j++) {
                Rgba32 rgba = image2[j, i];
                rgba.A = mask[j, i].A;
                image2[j, i] = rgba;
            }
        }
    }

    private Image<Rgba32> ApplyBlurToImage(
        Image<Rgba32> src, 
        float blurSigma, 
        float blurUpperLayerOpacity,
        LinearGradientBrush[] brushes, 
        FrozenSet<QuantizedColor> palette) {
        if (brushes.Length == 0) {
            return src.Clone();
        }
        if (blurUpperLayerOpacity == 0f) {
            return src.Clone();
        }
        Image<Rgba32> result = src.Clone();
        Image<Rgba32>[] array = new Image<Rgba32>[brushes.Length];
        int num = palette.Count((QuantizedColor p) => p.IsDark);
        int num2 = palette.Count((QuantizedColor p) => !p.IsDark);
        bool flag = num >= num2;
        for (int i = 0; i < brushes.Length; i++) {
            Image<Rgba32> image = src.Clone();
            LinearGradientBrush brush = brushes[i];
            Color color = (flag ? DarkOverlayColor : LightOverlayColor).WithAlpha(blurUpperLayerOpacity);
            Image<Rgba32> materialColorLayer = new Image<Rgba32>(src.Width, src.Height, color.ToPixel<Rgba32>());
            try {
                using Image<Rgba32> image2 = new Image<Rgba32>(src.Width, src.Height, new Rgba32(Vector4.Zero));
                image.Mutate(delegate (IImageProcessingContext ctx)
                {
                    ctx.DrawImage(materialColorLayer, result.Bounds, PixelColorBlendingMode.Normal, PixelAlphaCompositionMode.SrcAtop, 1f);
                    ctx.GaussianBlur(blurSigma);
                });
                int drawThickness = image2.Width * image2.Height;
                Rectangle drawBounds = image2.Bounds;
                image2.Mutate(delegate (IImageProcessingContext ctx)
                {
                    ctx.Draw(brush, drawThickness, drawBounds);
                });
                ApplyOpacityMask(image, image2);
                array[i] = image;
            } finally {
                if (materialColorLayer != null) {
                    materialColorLayer.Dispose();
                }
            }
        }
        Image<Rgba32>[] array2 = array;
        foreach (Image<Rgba32> mask in array2) {
            result.Mutate(delegate (IImageProcessingContext ctx)
            {
                ctx.DrawImage(mask, result.Bounds, PixelColorBlendingMode.Normal, PixelAlphaCompositionMode.SrcAtop, 1f);
            });
            mask.Dispose();
        }

        return result;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _image = e.NameScope.Find<Image>("PART_Image");
        _progressRing = e.NameScope.Find<ProgressRing>("PART_ProgressRing");
    }

    protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == SourceProperty) {
            if (IsEnableBlur) {
                await InitBlurImageAsync();
            } else {
                var image = new Bitmap(Source);
                _image.Source = image;
            }
        }

        if (change.Property == IsEnableBlurProperty) {
            if (IsEnableBlur) {
                await InitBlurImageAsync();
            } else {
                var image = new Bitmap(Source);
                _image.Source = image;
            }
        }

        async ValueTask InitBlurImageAsync() {
            if (Source is null) {
                return;
            }

            var imageSourceSI = await SixLabors.ImageSharp.Image.LoadAsync<Rgba32>(Source);
            var brush = new LinearGradientBrush(new PointF(1080 / 2f, 0f),
                new PointF(1080 / 2f, imageSourceSI.Height),
                GradientRepetitionMode.None,
                new ColorStop(0.001f, in Color.White),
                new ColorStop(0.03f, in Color.White),
                new ColorStop(0.2f, in Color.Transparent),
                new ColorStop(0f, in Color.Transparent),
                new ColorStop(0.75f, in Color.White),
                new ColorStop(1f, in Color.White));

            using var srcHalf = imageSourceSI.Clone();
            FrozenSet<QuantizedColor> palette = srcHalf.GetPaletteFromBitmap()
                .OrderByDescending(c => c.Population)
                .ToFrozenSet();

            var bitmap = await Task.Run(() => {
                var image = ApplyBlurToImage(imageSourceSI, 15f, 0.5f, [brush], palette);
                return image.ToBitmap();
            });
            await Dispatcher.UIThread.InvokeAsync(() => _image.Source = bitmap);
        }
    }
}

public readonly struct QuantizedColor {
    public Avalonia.Media.Color Color { get; }

    public bool IsDark { get; }

    public int Population { get; }

    public QuantizedColor(Avalonia.Media.Color color, int population) {
        Color = color;
        Population = population;
        IsDark = CalculateYiqLuma(color) < 128;
    }

    private static int CalculateYiqLuma(Avalonia.Media.Color color) {
        return Convert.ToInt32(Math.Round((299 * color.R + 587 * color.G + 114 * color.B) / 1000f));
    }
}