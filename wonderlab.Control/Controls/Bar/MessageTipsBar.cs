using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.control.Animation;
using wonderlab.control.Controls.Dialog;
using wonderlab.control.Interface;

namespace wonderlab.control.Controls.Bar {
    /// <summary>
    /// 消息提示框
    /// </summary>
    public class MessageTipsBar : ListBoxItem {
        private Border layout = default!;
        private Button closeButton = default!;
        private Button gotoButton = default!;
        private Border messageContentLayout = default!;
        
        public string? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        public string? Message { get => GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
        public string? Time { get => GetValue(TimeProperty); set => SetValue(TimeProperty, value); }
        public bool IsOpen { get; set; } = true;

        //Event
        /// <summary>
        /// 点击时触发的事件
        /// </summary>
        public event EventHandler<EventArgs> MessageClicked;

        /// <summary>
        /// 打开后触发的事件
        /// </summary>
        public event EventHandler<EventArgs> Opened;

        public delegate void HideOfRunAction();
        public HideOfRunAction HideOfRun { get; set; }

        //Property
        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<MessageTipsBar, string>(nameof(Title), "Info");

        public static readonly StyledProperty<string> MessageProperty =
            AvaloniaProperty.Register<MessageTipsBar, string>(nameof(Message), "Some Message in the MessageTipsBar");

        public static readonly StyledProperty<string> TimeProperty =
            AvaloniaProperty.Register<MessageTipsBar, string>(nameof(Time), "11:45");

        public void ShowDialog() {
            IsOpen = true;
            OffsetChangeAnimation animation = new();
            animation.FromVerticalOffset = -120;
            animation.RunAnimation(this);

            animation.AnimationCompleted += (_, _) => Opened.Invoke(this, new());
        }

        public void HideDialog() {
            if (IsOpen) {
                IsOpen = false;

                IsHitTestVisible = false;
                OpacityChangeAnimation animation = new(true);
                animation.RunAnimation(this);
            }
        }

        private async void OnClick(object? sender, RoutedEventArgs e) {
            if (IsOpen) {
                IsOpen = false;
                IsHitTestVisible = false;
                layout.Opacity = 0;
                Margin = new(0, 0, -430, 0);
                if ((sender as Button)?.Name is "GotoButton") {
                    await Task.Delay(275);
                    HideOfRun?.Invoke();
                } else
                    await Task.Delay(275);

                App.Cache.Remove(this);
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);
            layout = e.NameScope.Find<Border>("Layout")!;
            gotoButton = e.NameScope.Find<Button>("GotoButton")!;
            closeButton = e.NameScope.Find<Button>("CloseButton")!;
            messageContentLayout = e.NameScope.Find<Border>("MessageContentLayout")!;
            
            layout.Opacity = 0;
            Margin = new(0, 0, -430, 0); 
            closeButton.Click += OnClick;
            gotoButton.Click += OnClick;
            PointerEntered += OnPointerEntered;
            PointerExited += OnPointerExited;
        }

        private void OnPointerEntered(object? sender, Avalonia.Input.PointerEventArgs e) {
            if(HideOfRun is null) {
                messageContentLayout.Width = 348;
            } else {
                messageContentLayout.Width = 318;
            }
        }

        private void OnPointerExited(object? sender, Avalonia.Input.PointerEventArgs e) {
            messageContentLayout.Width = 378;
        }
        
        protected override async void OnLoaded(RoutedEventArgs e) {
            base.OnLoaded(e);
            Margin = new(0, 0, 0, 0);
            layout.Opacity = 1;
            await Task.Delay(4000);
            Opacity = 0;
            Margin = new(0, 0, -430, 0);
            await Task.Delay(150);
            App.Cache.Remove(this);
        }

        public MessageTipsBar() {
        }

        public MessageTipsBar(HideOfRunAction action) {
            HideOfRun = action;
        }
    }
}
