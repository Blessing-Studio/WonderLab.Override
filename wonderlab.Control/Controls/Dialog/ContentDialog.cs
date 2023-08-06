using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
        private Border BackgroundBorder = null!;

        private ContentControl control = null!;

        public bool IsOpen { get => GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }

        //Event
        public event EventHandler<EventArgs>? ButtonClick;

        //Property
        public static readonly StyledProperty<bool> IsOpenProperty =
            AvaloniaProperty.Register<MessageDialog, bool>(nameof(IsOpen), false);

        public ContentDialog() {       
            IsOpen = false;
        }

        public void HideDialog()
        {
            BackgroundBorder.IsHitTestVisible = false;
            OpacityChangeAnimation animation = new(true);
            animation.RunAnimation(BackgroundBorder);
            animation.AnimationCompleted += (_, _) => { control.IsHitTestVisible = false; IsVisible = false; };
            animation.RunAnimation(control);
        }

        public void ShowDialog()
        {
            IsVisible = true;
            BackgroundBorder.IsHitTestVisible = true;

            OpacityChangeAnimation animation = new(false) {
                RunValue = 0
            };
            animation.RunAnimation(BackgroundBorder);
            animation.AnimationCompleted += (_, _) => control.IsHitTestVisible = true;
            animation.RunAnimation(control);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            BackgroundBorder = e.NameScope.Find<Border>("BackgroundBorder");
            control = e.NameScope.Find<ContentControl>("content");
            IsVisible = false;

            BackgroundBorder.PointerPressed += (_, args) => {
                Manager.Current.BeginMoveDrag(args);
            };
        }

        private void OnCloseButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ButtonClick?.Invoke(sender, new());
        }
    }
}
