using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using wonderlab.control.Animation;
using wonderlab.control.Controls.Dialog.Events;
using wonderlab.control.Interface;

namespace wonderlab.control.Controls.Dialog
{
    /// <summary>
    /// 信息对话框
    /// </summary>
    [PseudoClasses(":open", ":close")]
    public class MessageDialog : ContentControl, IDialog, IMessageDialog
    {
        Button CloseButton = null!;        
        Border BackgroundBorder = null!;
        Border DialogContent = null!;
        
        public bool IsOpen { get => GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
        public bool Button1Visible { get => GetValue(Button1VisibleProperty); set => SetValue(Button1VisibleProperty, value); }
        public bool Button2Visible { get => GetValue(Button2VisibleProperty); set => SetValue(Button2VisibleProperty, value); }
        public bool Button3Visible { get => GetValue(Button3VisibleProperty); set => SetValue(Button3VisibleProperty, value); }
        public string? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        public string? Message { get => GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
        public string? Button1Text { get => GetValue(Button1TextProperty); set => SetValue(Button1TextProperty, value); }
        public string? Button2Text { get => GetValue(Button2TextProperty); set => SetValue(Button2TextProperty, value); }
        public string? Button3Text { get => GetValue(Button3TextProperty); set => SetValue(Button3TextProperty, value); }

        //Event
        public event EventHandler<CloseButtonClick>? CloseButtonClick;
        public event EventHandler<EventArgs>? Button2Click;
        public event EventHandler<EventArgs>? Button3Click;

        //Property
        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<MessageDialog, string>(nameof(Title), "信息");

        public static readonly StyledProperty<string> MessageProperty =
            AvaloniaProperty.Register<MessageDialog, string>(nameof(Message), "这是一段好长的信息啊啊啊啊啊啊啊啊啊啊啊啊啊");

        public static readonly StyledProperty<string> Button1TextProperty =
            AvaloniaProperty.Register<MessageDialog, string>(nameof(Button1Text));

        public static readonly StyledProperty<string> Button2TextProperty =
            AvaloniaProperty.Register<MessageDialog, string>(nameof(Button2Text));

        public static readonly StyledProperty<string> Button3TextProperty =
            AvaloniaProperty.Register<MessageDialog, string>(nameof(Button3Text));

        public static readonly StyledProperty<bool> Button1VisibleProperty =
            AvaloniaProperty.Register<MessageDialog, bool>(nameof(Button1Visible), true);

        public static readonly StyledProperty<bool> Button2VisibleProperty =
            AvaloniaProperty.Register<MessageDialog, bool>(nameof(Button2Visible), true);

        public static readonly StyledProperty<bool> Button3VisibleProperty =
            AvaloniaProperty.Register<MessageDialog, bool>(nameof(Button3Visible), true);

        public static readonly StyledProperty<bool> IsOpenProperty =
            AvaloniaProperty.Register<MessageDialog, bool>(nameof(IsOpen), false);

        public MessageDialog() {
            IsOpen = false;
        }

        public void HideDialog() {
            BackgroundBorder.IsHitTestVisible = false;
            DialogContent.IsHitTestVisible = false;
            OpacityChangeAnimation animation = new(true);
            animation.RunAnimation(BackgroundBorder);
            animation.RunAnimation(DialogContent);
        }

        public void ShowDialog() {
            BackgroundBorder.IsHitTestVisible = true;
            DialogContent.IsHitTestVisible = true;

            OpacityChangeAnimation animation = new(false) {
                RunValue = 0
            };
            animation.RunAnimation(BackgroundBorder);
            animation.RunAnimation(DialogContent);
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
            DialogContent = e.NameScope.Find<Border>("DialogContent");
            CloseButton = e.NameScope.Find<Button>("CloseButton");
            CloseButton.Click += OnCloseButtonClick;

            HideDialog();
        }

        private void OnCloseButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            CloseButtonClick?.Invoke(sender, new());
        }
    }
}
