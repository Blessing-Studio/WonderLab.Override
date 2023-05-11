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
    /// 信息对话框 -别tm改这个页面里的东西了啊啊啊啊啊啊啊啊啊啊
    /// </summary>
    [PseudoClasses(":open", ":close")]
    public class UpdateDialog : ContentControl, IDialog
    {
        Grid Buttons = null!;
        Border BackgroundBorder = null!;
        Border DialogContent = null!;
        Grid Bar = null!;

        public bool IsOpen { get => GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
        public bool Button1Visible { get => GetValue(Button1VisibleProperty); set => SetValue(Button1VisibleProperty, value); }
        public bool Button2Visible { get => GetValue(Button2VisibleProperty); set => SetValue(Button2VisibleProperty, value); }
        public string? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        public object? Message { get => GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
        public string? Button1Text { get => GetValue(Button1TextProperty); set => SetValue(Button1TextProperty, value); }
        public string? Button2Text { get => GetValue(Button2TextProperty); set => SetValue(Button2TextProperty, value); }
        public double UpdateProgress { get => GetValue(UpdateProgressProperty); set => SetValue(UpdateProgressProperty, value); }

        public bool HasUpdate { get; set; }
        //Event
        public event EventHandler<EventArgs>? AcceptButtonClick;
        public event EventHandler<EventArgs>? CloseButtonClick;

        //Property
        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<UpdateDialog, string>(nameof(Title), "好耶，更新力");

        public static readonly StyledProperty<object> MessageProperty =
            AvaloniaProperty.Register<UpdateDialog, object>(nameof(Message), "");

        public static readonly StyledProperty<string> Button1TextProperty =
            AvaloniaProperty.Register<UpdateDialog, string>(nameof(Button1Text), "或者更新");//别改！！！！这是强制更新！！！

        public static readonly StyledProperty<string> Button2TextProperty =
            AvaloniaProperty.Register<UpdateDialog, string>(nameof(Button2Text), "立刻更新");

        public static readonly StyledProperty<bool> Button1VisibleProperty =
            AvaloniaProperty.Register<UpdateDialog, bool>(nameof(Button1Visible), true);

        public static readonly StyledProperty<bool> Button2VisibleProperty =
            AvaloniaProperty.Register<UpdateDialog, bool>(nameof(Button2Visible), true);

        public static readonly StyledProperty<double> UpdateProgressProperty =
            AvaloniaProperty.Register<UpdateDialog, double>(nameof(Button2Visible), 0.0);

        public static readonly StyledProperty<bool> IsOpenProperty =
            AvaloniaProperty.Register<UpdateDialog, bool>(nameof(IsOpen), false);

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

            OpacityChangeAnimation animation = new(false){
                RunValue = 0
            };
            animation.RunAnimation(BackgroundBorder);
            animation.RunAnimation(DialogContent);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            
            BackgroundBorder = e.NameScope.Find<Border>("BackgroundBorder");
            Bar = e.NameScope.Find<Grid>("UpP");
            DialogContent = e.NameScope.Find<Border>("DialogContent");
            Buttons = e.NameScope.Find<Grid>("Buttons");
            e.NameScope.Find<Button>("CloseButton").Click += OnCloseButtonClick;
            e.NameScope.Find<Button>("Button2").Click += OnCloseButtonClick;

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
            CloseButtonClick?.Invoke(sender, new());
        }
    }
}
