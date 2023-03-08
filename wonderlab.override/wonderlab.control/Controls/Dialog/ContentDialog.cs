using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.control.Animation;
using wonderlab.control.Interface;

namespace wonderlab.control.Controls.Dialog
{
    /// <summary>
    /// 信息对话框
    /// </summary>
    [PseudoClasses(":open", ":close")]
    public class ContentDialog : ContentControl, IDialog
    {
        Border BackgroundBorder = null!;

        public bool IsOpen { get => GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }

        //Event
        public event EventHandler<EventArgs>? ButtonClick;

        //Property
        public static readonly StyledProperty<bool> IsOpenProperty =
            AvaloniaProperty.Register<MessageDialog, bool>(nameof(IsOpen), false);

        public ContentDialog()
        {
            IsOpen = false;
        }

        public void HideDialog()
        {
            BackgroundBorder.IsHitTestVisible = false;
            OpacityChangeAnimation animation = new(true);
            animation.RunAnimation(BackgroundBorder);
        }

        public void ShowDialog()
        {
            BackgroundBorder.IsHitTestVisible = true;

            OpacityChangeAnimation animation = new(false);
            animation.RunAnimation(BackgroundBorder);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == IsOpenProperty && IsOpen is false)
            {
                PseudoClasses.Set(":close", e.NewValue.GetValueOrDefault<bool>());
            }
            else if (e.Property == IsOpenProperty)
            {
                PseudoClasses.Set(":open", e.NewValue.GetValueOrDefault<bool>());
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            BackgroundBorder = e.NameScope.Find<Border>("BackgroundBorder");
        }

        private void OnCloseButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ButtonClick?.Invoke(sender, new());
        }
    }
}
