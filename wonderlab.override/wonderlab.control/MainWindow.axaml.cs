using Avalonia.Controls;
using System.Diagnostics;
using System.Threading.Tasks;
using wonderlab.control.Controls.Bar;
using wonderlab.control.Theme;

namespace wonderlab.control
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Opened += MainWindow_Opened;
            InitializeComponent();
            ColorHelper helper = new ColorHelper();
            helper.Load();
            //d.CloseButtonClick += D_CloseButtonClick;
            button.Click += Button_Click;
            ThemeChange.Checked += ThemeChange_Checked;
            bab.LaunchButtonClick += Bab_LaunchButtonClick;
        }

        private void Bab_LaunchButtonClick(object? sender, System.EventArgs e)
        {
            MessageTipsBar bar = new MessageTipsBar(new(Action))
            {
                Title = "信息",
                Message = "开始尝试启动游戏 1.12.2-WonderLab-DemoClient"
            };             
            grid.Children.Add(bar);
            bar.Opened += async (_, _) =>
            {
                await Task.Delay(3000);
                bar.HideDialog();
            };

            bar.ShowDialog();
        }

        private void ThemeChange_Checked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ColorHelper helper = new();
            if (ThemeChange.IsChecked!.Value)
            {
                helper.UpdateTheme("Light");
            }
            else
            {
                helper.UpdateTheme("Dark");
            }
        }

        void Action()
        {
            Trace.WriteLine("[Info] This MessageTipsBar is Clicked!");
        }

        private async void MainWindow_Opened(object? sender, System.EventArgs e)
        {
            await Task.Delay(1000);
            tb.InitStartAnimation();

        }

        private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            d.ShowDialog();
        }

        private void D_CloseButtonClick(object? sender, Controls.Dialog.Events.CloseButtonClick e)
        {
        }

        private void ShowDialogClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MessageTipsBar bar = new MessageTipsBar(new(Action));
            grid.Children.Add(bar);
            bar.Opened += async (_, _) =>
            {
                await Task.Delay(1000);
                bar.HideDialog();
            };

            bar.ShowDialog();
        }
    }
}
