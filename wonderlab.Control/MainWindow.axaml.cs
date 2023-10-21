using Avalonia.Controls;
using System.Diagnostics;
using System.Threading.Tasks;

namespace wonderlab.control {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Test.Click += Test_Click;
            Test1.Click += Open_Click;
        }

        private async void Open_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            count++;
            tbv.Add("测试标题", $"第 {count} 次被触发", () => {
                Trace.WriteLine($"第 {count} 次被触发");
            });
        }

        int count;
        private async void Test_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            count++;
            tbv.Add("测试标题", $"第 {count} 次被触发");
        }
    }
}