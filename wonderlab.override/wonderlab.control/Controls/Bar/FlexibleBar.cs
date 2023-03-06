using Avalonia;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace wonderlab.control.Controls.Bar
{
    public class FlexibleBar : TemplatedControl {
        public FlexibleBar() { 
            
        }

        public bool IsOpen { get => GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }

        public double? RenderTransformValue { get => GetValue(RenderTransformValueProperty); set => SetValue(RenderTransformValueProperty, value); }

        public double? OpenHeight { get => GetValue(OpenHeightProperty); set => SetValue(OpenHeightProperty, value); }

        //Property
        public static readonly StyledProperty<bool> IsOpenProperty =
               AvaloniaProperty.Register<FlexibleBar, bool>(nameof(IsOpen), false);

        public static readonly StyledProperty<double> RenderTransformValueProperty =
               AvaloniaProperty.Register<FlexibleBar, double>(nameof(IsOpen));

        public static readonly StyledProperty<double> OpenHeightProperty =
               AvaloniaProperty.Register<FlexibleBar, double>(nameof(IsOpen));

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {       
            base.OnApplyTemplate(e);


        }
    }
}
