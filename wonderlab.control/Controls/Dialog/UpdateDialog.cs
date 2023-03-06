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
using wonderlab.control.Controls.Dialog.Events;
using wonderlab.control.Interface;

namespace wonderlab.control.Controls.Dialog
{
    /// <summary>
    /// 信息对话框
    /// </summary>
    [PseudoClasses(":open", ":close")]
    public class UpdateDialog : ContentControl, IDialog, IMessageDialog
    {
        Button CloseButton = null!;
        StackPanel Buttons = null!;
        Border BackgroundBorder = null!;
        Border DialogContent = null!;
        ProgressBar Bar = null!;

        public bool IsOpen { get => GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
        public bool Button1Visible { get => GetValue(Button1VisibleProperty); set => SetValue(Button1VisibleProperty, value); }
        public bool Button2Visible { get => GetValue(Button2VisibleProperty); set => SetValue(Button2VisibleProperty, value); }
        public string? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        public string? Message { get => GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
        public string? Button1Text { get => GetValue(Button1TextProperty); set => SetValue(Button1TextProperty, value); }
        public string? Button2Text { get => GetValue(Button2TextProperty); set => SetValue(Button2TextProperty, value); }
        public double UpdateProgress { get => GetValue(UpdateProgressProperty); set => SetValue(UpdateProgressProperty, value); }

        public bool HasUpdate { get; set; }
        //Event
        public event EventHandler<EventArgs>? ButtonClick;

        //Property
        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<MessageDialog, string>(nameof(Title), "好耶，更新力");

        public static readonly StyledProperty<string> MessageProperty =
            AvaloniaProperty.Register<MessageDialog, string>(nameof(Message), "阿巴阿巴阿巴阿巴阿巴阿阿巴阿巴阿巴阿巴阿巴阿巴巴");

        public static readonly StyledProperty<string> Button1TextProperty =
            AvaloniaProperty.Register<MessageDialog, string>(nameof(Button1Text), "或者更新");

        public static readonly StyledProperty<string> Button2TextProperty =
            AvaloniaProperty.Register<MessageDialog, string>(nameof(Button2Text), "立刻更新");

        public static readonly StyledProperty<bool> Button1VisibleProperty =
            AvaloniaProperty.Register<MessageDialog, bool>(nameof(Button1Visible), true);

        public static readonly StyledProperty<bool> Button2VisibleProperty =
            AvaloniaProperty.Register<MessageDialog, bool>(nameof(Button2Visible), true);

        public static readonly StyledProperty<double> UpdateProgressProperty =
            AvaloniaProperty.Register<MessageDialog, double>(nameof(Button2Visible), 0.0);

        public static readonly StyledProperty<bool> IsOpenProperty =
            AvaloniaProperty.Register<MessageDialog, bool>(nameof(IsOpen), false);

        public UpdateDialog() {        
            IsOpen = false;
        }

        public void HideDialog()
        {
            BackgroundBorder.IsHitTestVisible = false;
            DialogContent.IsHitTestVisible = false;
            OpacityChangeAnimation animation = new(true);
            animation.RunAnimation(BackgroundBorder);
            animation.RunAnimation(DialogContent);
        }

        public void ShowDialog()
        {
            HasUpdate = true;
            BackgroundBorder.IsHitTestVisible = true;
            DialogContent.IsHitTestVisible = true;

            OpacityChangeAnimation animation = new(false);
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
            Bar = e.NameScope.Find<ProgressBar>("UpP");
            DialogContent = e.NameScope.Find<Border>("DialogContent");
            CloseButton = e.NameScope.Find<Button>("Button2");
            Buttons = e.NameScope.Find<StackPanel>("Buttons");
            e.NameScope.Find<Button>("CloseButton").Click += OnCloseButtonClick;
            CloseButton.Click += OnCloseButtonClick;

            if (!HasUpdate)
            {
                HideDialog();
            }
        }

        public void StartInit()
        {
            Buttons.IsVisible = false;
            Bar.IsVisible = true;
        }

        private void OnCloseButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            StartInit();
            ButtonClick?.Invoke(sender, new());
        }
    }
}
