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

namespace wonderlab.control.Controls.Dialog {
    /// <summary>
    /// 信息对话框
    /// </summary>
    [PseudoClasses(":open", ":close")]
    public class MessageDialog : ContentControl, IMessageDialog {
        private Button CloseButton = null!;

        private Border BackgroundBorder = null!;

        private Border DialogContent = null!;

        public bool IsOpen { get => GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }

        public bool CloseButtonVisible { get => GetValue(CloseButtonVisibleProperty); set => SetValue(CloseButtonVisibleProperty, value); }

        public bool CustomButtonVisible { get => GetValue(CustomButtonVisibleProperty); set => SetValue(CustomButtonVisibleProperty, value); }

        public string? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty!, value); }                         

        public string? Message { get => GetValue(MessageProperty); set => SetValue(MessageProperty!, value); }

        public string CloseButtonText { get => GetValue(CloseButtonTextProperty); set => SetValue(CloseButtonTextProperty, value); }

        public string CustomButtonText { get => GetValue(CustomButtonTextProperty); set => SetValue(CustomButtonTextProperty, value); }

        //Event
        public event EventHandler<CloseButtonClick>? CloseButtonClick;

        public event EventHandler<EventArgs>? CustomButtonClick;

        //Property
        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<MessageDialog, string>(nameof(Title), "信息");

        public static readonly StyledProperty<string> MessageProperty =
            AvaloniaProperty.Register<MessageDialog, string>(nameof(Message), "这是一段好长的信息啊啊啊啊啊啊啊啊啊啊啊啊啊");

        public static readonly StyledProperty<string> CloseButtonTextProperty =
            AvaloniaProperty.Register<MessageDialog, string>(nameof(CloseButtonText));

        public static readonly StyledProperty<string> CustomButtonTextProperty =
            AvaloniaProperty.Register<MessageDialog, string>(nameof(CustomButtonText));

        public static readonly StyledProperty<bool> CloseButtonVisibleProperty =
            AvaloniaProperty.Register<MessageDialog, bool>(nameof(CloseButtonVisible), true);

        public static readonly StyledProperty<bool> CustomButtonVisibleProperty =
            AvaloniaProperty.Register<MessageDialog, bool>(nameof(CustomButtonVisible), true);

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

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);

            BackgroundBorder = e.NameScope.Find<Border>("BackgroundBorder")!;
            DialogContent = e.NameScope.Find<Border>("DialogContent")!;
            CloseButton = e.NameScope.Find<Button>("CloseButton")!;
            CloseButton.Click += OnCloseButtonClick;
            e.NameScope.Find<Button>("CustomButton")!.Click += OnCustomButtonClick;

            BackgroundBorder.PointerPressed += (_, args) => {
                Manager.Current.BeginMoveDrag(args);
            };

            HideDialog();
        }

        private void OnCustomButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            CustomButtonClick?.Invoke(sender, e);
        }

        private void OnCloseButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            HideDialog();
            CloseButtonClick?.Invoke(sender, new());
        }
    }
}