using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;

namespace WonderLab.Views.Controls;

public class SmoothBorder : TemplatedControl {
    private Path _path = default!;

    public static readonly StyledProperty<object> ContentProperty =
        AvaloniaProperty.Register<SmoothBorder, object>(nameof(Content));

    public object Content {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    private StreamGeometry HandleSmoothRoundedGeometry(Rect rect, CornerRadius cornerRadius) {
        var width = rect.Width;
        var height = rect.Height;
        StreamGeometry geometry = new StreamGeometry();

        using var context = geometry.Open();
        context.BeginFigure(new Point(cornerRadius.TopLeft + rect.Left, rect.Top), true);

        context.LineTo(new Point(width - cornerRadius.TopRight + rect.Left, rect.Top));
        context.CubicBezierTo(
            new Point(width - cornerRadius.TopRight + cornerRadius.TopRight * 2 / 3 + rect.Left, rect.Top),
            new Point(width + rect.Left, cornerRadius.TopRight - cornerRadius.TopRight * 2 / 3 + rect.Top),
            new Point(width + rect.Left, cornerRadius.TopRight + rect.Top));

        context.LineTo(new Point(width + rect.Left, height - cornerRadius.BottomRight + rect.Top));
        context.CubicBezierTo(
            new Point(width + rect.Left, height - cornerRadius.BottomRight + cornerRadius.BottomRight * 2 / 3 + rect.Top),
            new Point(width - cornerRadius.BottomRight + cornerRadius.BottomRight * 2 / 3 + rect.Left, height + rect.Top),
            new Point(width - cornerRadius.BottomRight + rect.Left, height + rect.Top));

        context.LineTo(new Point(cornerRadius.BottomLeft + rect.Left, height + rect.Top));
        context.CubicBezierTo(
            new Point(cornerRadius.BottomLeft - cornerRadius.BottomLeft * 2 / 3 + rect.Left, height + rect.Top),
            new Point(rect.Left, height - cornerRadius.BottomLeft + cornerRadius.BottomLeft * 2 / 3 + rect.Top),
            new Point(rect.Left, height - cornerRadius.BottomLeft + rect.Top));

        context.LineTo(new Point(rect.Left, cornerRadius.TopLeft + rect.Top));
        context.CubicBezierTo(
            new Point(rect.Left, cornerRadius.TopLeft - cornerRadius.TopLeft * 2 / 3 + rect.Top),
            new Point(cornerRadius.TopLeft - cornerRadius.TopLeft * 2 / 3 + rect.Left, rect.Top),
            new Point(cornerRadius.TopLeft + rect.Left, rect.Top));

        return geometry;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
            
        _path = e.NameScope.Find<Path>("backgroundPath")!;
        _path!.Data = HandleSmoothRoundedGeometry(Bounds, CornerRadius);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);
        if ((change.Property == CornerRadiusProperty || change.Property == BoundsProperty) && _path is not null) {
            _path!.Data = HandleSmoothRoundedGeometry(Bounds, CornerRadius);
        }
    }
}