using System;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Controls;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Automation.Peers;

namespace WonderLab.Views.Controls;

[TemplatePart("PART_Indicator", typeof(Border))]
[PseudoClasses(":vertical", ":horizontal", ":indeterminate")]
public class MacOsProgressBar : ProgressBar {
    private double _percentage;

    private Border? _indicator;
    private IDisposable? _trackSizeChangedListener;

    public double Percentage {
        get => _percentage;
        private set => SetAndRaise(PercentageProperty, ref _percentage, value);
    }

    protected override Size ArrangeOverride(Size finalSize) {
        var result = base.ArrangeOverride(finalSize);
        UpdateIndicator();
        return result;
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty ||
            change.Property == MinimumProperty ||
            change.Property == MaximumProperty ||
            change.Property == IsIndeterminateProperty ||
            change.Property == OrientationProperty) {
            UpdateIndicator();
        }

        if (change.Property == IsIndeterminateProperty) {
            UpdatePseudoClasses(change.GetNewValue<bool>(), null);
        } else if (change.Property == OrientationProperty) {
            UpdatePseudoClasses(null, change.GetNewValue<Orientation>());
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        // dispose any previous track size listener
        _trackSizeChangedListener?.Dispose();

        _indicator = e.NameScope.Get<Border>("PART_Indicator");

        // listen to size changes of the indicators track (parent) and update the indicator there. 
        _trackSizeChangedListener = _indicator.Parent?.GetPropertyChangedObservable(BoundsProperty)
            .Subscribe(_ => UpdateIndicator());

        UpdateIndicator();
    }

    protected override AutomationPeer OnCreateAutomationPeer() {
        return new ProgressBarAutomationPeer(this);
    }

    private void UpdateIndicator() {
        // Gets the size of the parent indicator container
        var barSize = (_indicator?.Parent as Control)?.Bounds.Size ?? 
                      Bounds.Size;

        if (_indicator == null) return;
        if (IsIndeterminate) {
            var dim = Orientation == Orientation.Horizontal ? barSize.Width : barSize.Height;
            var barIndicatorWidth = dim * 0.4; // Indicator width at 40% of ProgressBar
            var barIndicatorWidth2 = dim * 0.6; // Indicator width at 60% of ProgressBar

            TemplateSettings.ContainerWidth = barIndicatorWidth;
            TemplateSettings.Container2Width = barIndicatorWidth2;

            TemplateSettings.Container2AnimationStartPosition = barIndicatorWidth2 * -1.085;
            TemplateSettings.Container2AnimationEndPosition = barIndicatorWidth2 * 1.085;

            TemplateSettings.IndeterminateStartingOffset = -dim;
            TemplateSettings.IndeterminateEndingOffset = dim;
        } else {
            var percent = Math.Abs(Maximum - Minimum) < double.Epsilon ? 1.0 :
                (Value - Minimum) / (Maximum - Minimum);

            if (Orientation == Orientation.Horizontal) {
                _indicator.Width = (barSize.Width - _indicator.Margin.Left - _indicator.Margin.Right) * percent;
                _indicator.Height = double.NaN;
            } else {
                _indicator.Width = double.NaN;
                _indicator.Height = (barSize.Height - _indicator.Margin.Top - _indicator.Margin.Bottom) 
                                    * percent;
            }


            Percentage = percent * 100;
        }
    }

    private void UpdatePseudoClasses(
        bool? isIndeterminate,
        Orientation? o) {
        if (isIndeterminate.HasValue) {
            PseudoClasses.Set(":indeterminate", isIndeterminate.Value);
        }

        if (!o.HasValue) return;
        PseudoClasses.Set(":vertical", o == Orientation.Vertical);
        PseudoClasses.Set(":horizontal", o == Orientation.Horizontal);
    }
}