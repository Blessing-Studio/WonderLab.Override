using Avalonia;
using Avalonia.Controls;
using System.Linq;
using System;

namespace WonderLab.Views.Controls;

///From:https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/main/Microsoft.Toolkit.Uwp.UI.Controls.Primitives/StaggeredPanel/StaggeredPanel.cs

/// <summary>
/// Arranges child elements into a staggered grid pattern where items are added to the column that has used least amount of space.
/// </summary>
public class StaggeredPanel : Panel {
    private double _columnWidth;

    public static readonly StyledProperty<double> DesiredColumnWidthProperty =
        AvaloniaProperty.Register<StaggeredPanel, double>(nameof(DesiredColumnWidth), 250d);

    public static readonly StyledProperty<Thickness> PaddingProperty =
        AvaloniaProperty.Register<StaggeredPanel, Thickness>(nameof(Padding), new Thickness(0));

    public static readonly StyledProperty<double> ColumnSpacingProperty =
        AvaloniaProperty.Register<StaggeredPanel, double>(nameof(ColumnSpacing), 0d);

    public static readonly StyledProperty<double> RowSpacingProperty =
        AvaloniaProperty.Register<StaggeredPanel, double>(nameof(RowSpacing), 0d);

    public double DesiredColumnWidth {
        get => GetValue(DesiredColumnWidthProperty);
        set => SetValue(DesiredColumnWidthProperty, value);
    }

    public Thickness Padding {
        get => GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public double ColumnSpacing {
        get => GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
    }

    public double RowSpacing {
        get => GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    static StaggeredPanel() {
        AffectsMeasure<StaggeredPanel>(DesiredColumnWidthProperty, PaddingProperty, ColumnSpacingProperty, RowSpacingProperty);
    }

    protected override Size MeasureOverride(Size availableSize) {
        double availableWidth = availableSize.Width - Padding.Left - Padding.Right;
        double availableHeight = availableSize.Height - Padding.Top - Padding.Bottom;

        _columnWidth = Math.Min(DesiredColumnWidth, availableWidth);
        int numColumns = Math.Max(1, (int)Math.Floor(availableWidth / _columnWidth));

        double totalWidth = _columnWidth * numColumns + ColumnSpacing * (numColumns - 1);
        if (totalWidth > availableWidth) {
            numColumns--;
        }

        if (double.IsInfinity(availableWidth)) {
            availableWidth = totalWidth;
        }

        if (HorizontalAlignment == Avalonia.Layout.HorizontalAlignment.Stretch) {
            availableWidth = availableWidth - ColumnSpacing * (numColumns - 1);
            _columnWidth = availableWidth / numColumns;
        }

        var columnHeights = new double[numColumns];
        var itemsPerColumn = new double[numColumns];

        foreach (var child in Children) {
            var columnIndex = GetColumnIndex(columnHeights);

            child.Measure(new Size(_columnWidth, availableHeight));
            var elementSize = child.DesiredSize;
            columnHeights[columnIndex] += elementSize.Height + (itemsPerColumn[columnIndex] > 0 ? RowSpacing : 0);
            itemsPerColumn[columnIndex]++;
        }

        double desiredHeight = columnHeights.Max();
        return new Size(availableWidth, desiredHeight);
    }

    protected override Size ArrangeOverride(Size finalSize) {
        double horizontalOffset = Padding.Left;
        double verticalOffset = Padding.Top;
        int numColumns = Math.Max(1, (int)Math.Floor(finalSize.Width / _columnWidth));

        double totalWidth = _columnWidth * numColumns + ColumnSpacing * (numColumns - 1);
        if (totalWidth > finalSize.Width) {
            numColumns--;
        }

        if (HorizontalAlignment == Avalonia.Layout.HorizontalAlignment.Right) {
            horizontalOffset += finalSize.Width - totalWidth;
        } else if (HorizontalAlignment == Avalonia.Layout.HorizontalAlignment.Center) {
            horizontalOffset += (finalSize.Width - totalWidth) / 2;
        }

        var columnHeights = new double[numColumns];
        var itemsPerColumn = new double[numColumns];

        foreach (var child in Children) {
            var columnIndex = GetColumnIndex(columnHeights);

            var elementSize = child.DesiredSize;

            double elementHeight = elementSize.Height;

            double itemHorizontalOffset = horizontalOffset + (_columnWidth + ColumnSpacing) * columnIndex;
            double itemVerticalOffset = columnHeights[columnIndex] + verticalOffset + (RowSpacing * itemsPerColumn[columnIndex]);

            child.Arrange(new Rect(itemHorizontalOffset, itemVerticalOffset, _columnWidth, elementHeight));

            columnHeights[columnIndex] += elementSize.Height;
            itemsPerColumn[columnIndex]++;
        }

        double finalHeight = columnHeights.Max();
        return new Size(finalSize.Width, finalHeight);
    }

    private int GetColumnIndex(double[] columnHeights) {
        int columnIndex = 0;
        double height = columnHeights[0];
        for (int j = 1; j < columnHeights.Length; j++) {
            if (columnHeights[j] < height) {
                columnIndex = j;
                height = columnHeights[j];
            }
        }

        return columnIndex;
    }
}