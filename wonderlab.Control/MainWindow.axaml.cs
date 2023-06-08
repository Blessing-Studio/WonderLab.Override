using Avalonia.Controls;
using System.Threading.Tasks;

namespace wonderlab.control
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Initialized += MainWindow_Initialized;
            InitializeComponent();
            mic.Click += Start_Click;
            off.Click += Off_Click;
            ygg.Click += Ygg_Click;
        }

        private void Ygg_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            vd.ShowDialog(Controls.Dialog.ValidationDialog.ValidationTypes.Yggdrasil);
        }

        private void Off_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            vd.ShowDialog(Controls.Dialog.ValidationDialog.ValidationTypes.Offline);
        }

        private async void Start_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            vd.ShowDialog(Controls.Dialog.ValidationDialog.ValidationTypes.Microsoft);
            await Task.Delay(2000);
            vd.IsCodeLoading = false;
        }

        private async void MainWindow_Initialized(object? sender, System.EventArgs e) {
            await Task.Delay(2000);
            vd.IsCodeLoading = false;
        }
    }
}