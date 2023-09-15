using Avalonia.Controls;
using System.Diagnostics;
using System.Threading.Tasks;

namespace wonderlab.control {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Test.Click += Test_Click;
        }

        private async void Open_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        }

        int count;
        private async void Test_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            count++;
            tbv.Add("测试标题", $"第 {count} 次被触发");
        }
    }
}