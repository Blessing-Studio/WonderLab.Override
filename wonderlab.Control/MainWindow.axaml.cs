using Avalonia.Controls;
using System.Diagnostics;
using System.Threading.Tasks;

namespace wonderlab.control {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Hide.Click += Hide_Click;
            Show.Click += Show_Click;
            Action.Click += Action_Click;
        }

        private void Action_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            Trace.WriteLine(id.SelectedLoader);
        }

        private void Show_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            id.ShowDialog();
        }

        private void Hide_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            id.HideDialog();
        }
    }
}