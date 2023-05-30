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
    public class MessageDialog : ContentControl, IDialog, IMessageDialog {
        Button CloseButton = null!;
        Border BackgroundBorder = null!;
        Border DialogContent = null!;

        public bool IsOpen { get => GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
        public bool CloseButtonVisible { get => GetValue(CloseButtonVisibleProperty); set => SetValue(CloseButtonVisibleProperty, value); }
        public bool CustomButtonVisible { get => GetValue(CustomButtonVisibleProperty); set => SetValue(CustomButtonVisibleProperty, value); }
        public string? Title
        {
#pragma warning disable CS8620 // 由于引用类型的可为 null 性差异，实参不能用于形参。
            get => GetValue(TitleProperty); set => SetValue(TitleProperty, value);
#pragma warning restore CS8620 // 由于引用类型的可为 null 性差异，实参不能用于形参。
        }
#pragma warning disable CS8620 // 由于引用类型的可为 null 性差异，实参不能用于形参。

        public string? Message { get => GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
#pragma warning restore CS8620 // 由于引用类型的可为 null 性差异，实参不能用于形参。

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

            BackgroundBorder = e.NameScope.Find<Border>("BackgroundBorder");
            DialogContent = e.NameScope.Find<Border>("DialogContent");
            CloseButton = e.NameScope.Find<Button>("CloseButton");
            CloseButton.Click += OnCloseButtonClick;
            e.NameScope.Find<Button>("CustomButton").Click += OnCustomButtonClick; ;
            
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