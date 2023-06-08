using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.control.Animation;
using wonderlab.control.Controls.Dialog;
using wonderlab.control.Interface;

namespace wonderlab.control.Controls.Bar
{
    /// <summary>
    /// 消息提示框
    /// </summary>
    public class MessageTipsBar : Button,IDialog
    {
        public string? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        public string? Message { get => GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
        public string? Time { get => GetValue(TimeProperty); set => SetValue(TimeProperty, value); }
        public bool IsOpen { get; set; } = false;

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
            AvaloniaProperty.Register<MessageTipsBar, string>(nameof(Time), DateTime.Now.ToString(@"HH\:mm"));

        public void ShowDialog()
        {
            IsOpen = true;
            OffsetChangeAnimation animation = new();
            animation.FromVerticalOffset = -120;
            animation.RunAnimation(this);

            animation.AnimationCompleted += (_, _) => Opened.Invoke(this, new());
        }

        public void HideDialog()
        {
            if (IsOpen) {
                IsOpen = false;

                IsHitTestVisible = false;
                OpacityChangeAnimation animation = new(true);
                animation.RunAnimation(this);
            }
        }

        private void OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (IsOpen) {
                IsOpen = false;

                IsHitTestVisible = false;
                MessageTipsBarClickAnimation animation = new();
                animation.RunAnimation(this);                

                animation.AnimationCompleted += (_, _) => { if (HideOfRun is not null) HideOfRun(); };
            }
        }

        public MessageTipsBar() {
            Click += OnClick;
        }

        public MessageTipsBar(HideOfRunAction action) {
            HideOfRun = action;
            Click += OnClick;
        }
    }
}
