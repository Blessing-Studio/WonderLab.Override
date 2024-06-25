using System;
using Avalonia;
using System.Linq;
using Avalonia.Media;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Threading;
using WonderLab.Extensions;
using SixLabors.ImageSharp;
using WonderLab.Services.UI;
using Avalonia.Media.Imaging;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using System.Collections.Frozen;
using Avalonia.Controls.Primitives;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Color = SixLabors.ImageSharp.Color;
using SixLabors.ImageSharp.Drawing.Processing;
using Microsoft.Extensions.DependencyInjection;

using Size = Avalonia.Size;
using Point = Avalonia.Point;
using Rectangle = SixLabors.ImageSharp.Rectangle;

using Image = Avalonia.Controls.Image;
using LinearGradientBrush = SixLabors.ImageSharp.Drawing.Processing.LinearGradientBrush;
using Avalonia.LogicalTree;
using Avalonia.Interactivity;
using WonderLab.Services;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

namespace WonderLab.Views.Controls;

public sealed class ImageBox : TemplatedControl {
    private Image _image;
    private ProgressRing _progressRing;
    private WindowService _windowService;

    private readonly Color DarkOverlayColor = Color.ParseHex("#1C1C1C");
    private readonly Color LightOverlayColor = Color.ParseHex("#F6F6F6");

    public static readonly StyledProperty<string> SourceProperty =
        AvaloniaProperty.Register<ImageBox, string>(nameof(Source));

    public static readonly StyledProperty<int> BlurRadiusProperty =
        AvaloniaProperty.Register<ImageBox, int>(nameof(BlurRadius));

    public static readonly StyledProperty<bool> IsEnableBlurProperty =
        AvaloniaProperty.Register<ImageBox, bool>(nameof(IsEnableBlur));

    public static readonly StyledProperty<ParallaxMode> ParallaxModeProperty =
        AvaloniaProperty.Register<ImageBox, ParallaxMode>(nameof(ParallaxMode));

    public string Source {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public int BlurRadius {
        get => GetValue(BlurRadiusProperty);
        set => SetValue(BlurRadiusProperty, value);
    }

    public bool IsEnableBlur {
        get => GetValue(IsEnableBlurProperty);
        set => SetValue(IsEnableBlurProperty, value);
    }

    public ParallaxMode ParallaxMode {
        get => GetValue(ParallaxModeProperty);
        set => SetValue(ParallaxModeProperty, value);
    }

    private void SetDefaultFlatPosition() {
        if (_image.RenderTransform is not TranslateTransform) {
            return;
        }

        var translateTransform = (TranslateTransform)_image.RenderTransform;
        translateTransform.X = 0;
        translateTransform.Y = 0;
    }

    private void SetDefaultSolidPosition() {
        if (_image.RenderTransform is not Rotate3DTransform) {
            return;
        }

        var rotate3DTransform = (Rotate3DTransform)_image.RenderTransform;
        rotate3DTransform.Depth = 0.0;
        rotate3DTransform.AngleX = 0.0;
        rotate3DTransform.AngleY = 0.0;
        rotate3DTransform.CenterX = Bounds.Center.X;
        rotate3DTransform.CenterY = Bounds.Center.Y;
    }

    private void ApplySolidParallaxEffect(Point position) {
        Size desiredSize = _image.DesiredSize;
        double num = (position.X / desiredSize.Width - 0.5) * -5;
        double num2 = (position.Y / desiredSize.Height - 0.5) * -2;

        if (_image.RenderTransform is not Rotate3DTransform) {
            _image.RenderTransform = new Rotate3DTransform(num, num2, 0, num, num2, 0, 300) {
                Transitions = [new DoubleTransition {
                    Easing = new CubicEaseOut(),
                    Duration = TimeSpan.FromSeconds(0.35),
                    Property = Rotate3DTransform.DepthProperty,
                },
                new DoubleTransition {
                    Easing = new CubicEaseOut(),
                    Duration = TimeSpan.FromSeconds(0.35),
                    Property = Rotate3DTransform.CenterXProperty
                },
                new DoubleTransition {
                    Easing = new CubicEaseOut(),
                    Duration = TimeSpan.FromSeconds(0.35),
                    Property = Rotate3DTransform.CenterYProperty
                },
                new DoubleTransition {
                    Easing = new CubicEaseOut(),
                    Duration = TimeSpan.FromSeconds(0.35),
                    Property = Rotate3DTransform.AngleXProperty
                },
                new DoubleTransition {
                    Easing = new CubicEaseOut(),
                    Duration = TimeSpan.FromSeconds(0.35),
                    Property = Rotate3DTransform.AngleYProperty
                },]
            };

            return;
        }

        var rotate3DTransform = (Rotate3DTransform)_image.RenderTransform;
        rotate3DTransform.Depth = 300.0;
        rotate3DTransform.CenterX = num;
        rotate3DTransform.CenterY = num2;
        rotate3DTransform.AngleX = num;
        rotate3DTransform.AngleY = num2;
    }

    private void ApplyFlatParallaxEffect(Point position) {
        int xOffset = 50, yOffset = 50;

        Size desiredSize = _image.DesiredSize;
        double num = desiredSize.Height - position.X / xOffset - desiredSize.Height;
        double num2 = desiredSize.Width - position.Y / yOffset - desiredSize.Width;

        if (_image.RenderTransform is not TranslateTransform) {
            _image.RenderTransform = new TranslateTransform(num, num2) {
                Transitions = [new DoubleTransition {
                    Easing = new CubicEaseOut(),
                    Duration = TimeSpan.FromSeconds(0.35),
                    Property = TranslateTransform.XProperty
                },
                new DoubleTransition {
                    Easing = new CubicEaseOut(),
                    Duration = TimeSpan.FromSeconds(0.35),
                    Property = TranslateTransform.YProperty
                }]
            };

            return;
        }

        var translateTransform = (TranslateTransform)_image.RenderTransform;
        if (xOffset > 0) translateTransform.X = num;
        if (yOffset > 0) translateTransform.Y = num2;
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
                image.Mutate(delegate (IImageProcessingContext ctx) {
                    ctx.DrawImage(materialColorLayer, result.Bounds, PixelColorBlendingMode.Normal, PixelAlphaCompositionMode.SrcAtop, 1f);
                    ctx.GaussianBlur(blurSigma);
                });

                int drawThickness = image2.Width * image2.Height;
                Rectangle drawBounds = image2.Bounds;
                image2.Mutate(delegate (IImageProcessingContext ctx) {
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
            result.Mutate(delegate (IImageProcessingContext ctx) {
                ctx.DrawImage(mask, result.Bounds, PixelColorBlendingMode.Normal, PixelAlphaCompositionMode.SrcAtop, 1f);
            });

            mask.Dispose();
        }

        return result;
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        _windowService = App.ServiceProvider.GetService<WindowService>();
        if (ParallaxMode is ParallaxMode.None) {
            return;
        }

        if (ParallaxMode is ParallaxMode.Flat) {
            _image.Margin = new(-20);
            _windowService.RegisterPointerMoved(args => ApplyFlatParallaxEffect(args.GetPosition(_image)));
            _windowService.RegisterPointerExited(args => SetDefaultFlatPosition());
        } else {
            _image.Margin = new(-25);
            _windowService.RegisterPointerMoved(args => ApplySolidParallaxEffect(args.GetPosition(_image)));
            _windowService.RegisterPointerExited(args => SetDefaultSolidPosition());
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _image = e.NameScope.Find<Image>("PART_Image");
        _progressRing = e.NameScope.Find<ProgressRing>("PART_ProgressRing");
    }

    protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (!IsLoaded) {
            return;
        }

        if (change.Property == SourceProperty) {
            _progressRing.IsVisible = true;
            if (IsEnableBlur) {
                await InitBlurImageAsync();
            } else {
                var image = new Bitmap(Source);
                _image.Source = image;
            }

            _progressRing.IsVisible = false;
        }

        if (change.Property == IsEnableBlurProperty) {
            _progressRing.IsVisible = true;

            if (IsEnableBlur) {
                if (BlurRadius > 0) {
                    _image.Effect = new BlurEffect {
                        Radius = 0
                    };
                }

                await InitBlurImageAsync();
            } else {
                var image = new Bitmap(Source);
                _image.Source = image;

                if (BlurRadius > 0) {
                    _image.Effect = new BlurEffect {
                        Radius = BlurRadius
                    };
                }
            }

            _progressRing.IsVisible = false;
        }

        if (change.Property == BlurRadiusProperty) {
            if (IsEnableBlur) {
                return;
            }

            _image.Effect = new BlurEffect {
                Radius = BlurRadius
            };
        }

        if (change.Property == ParallaxModeProperty) {
            _image.Margin = new(0);
            _windowService.UnregisterPointerMoved();
            _windowService.UnregisterPointerExited();

            if (ParallaxMode is ParallaxMode.Flat) {
                _image.Margin = new(-20);
                _windowService.RegisterPointerExited(args => SetDefaultFlatPosition());
                _windowService.RegisterPointerMoved(args => ApplyFlatParallaxEffect(args.GetPosition(_image)));
                return;
            } else if (ParallaxMode is ParallaxMode.Solid) {
                _image.Margin = new(-35);
                _windowService.RegisterPointerExited(args => SetDefaultSolidPosition());
                _windowService.RegisterPointerMoved(args => ApplySolidParallaxEffect(args.GetPosition(_image)));
                return;
            }

            SetDefaultFlatPosition();
            SetDefaultSolidPosition();
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
    public bool IsDark { get; }
    public int Population { get; }
    public Avalonia.Media.Color Color { get; }

    public QuantizedColor(Avalonia.Media.Color color, int population) {
        Color = color;
        Population = population;
        IsDark = CalculateYiqLuma(color) < 128;
    }

    private static int CalculateYiqLuma(Avalonia.Media.Color color) {
        return Convert.ToInt32(Math.Round((299 * color.R + 587 * color.G + 114 * color.B) / 1000f));
    }
}