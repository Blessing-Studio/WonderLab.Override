using Avalonia.Controls;
using System.Diagnostics;
using System.Threading.Tasks;

namespace wonderlab.control {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Test.Click += Test_Click;
        }
        int count;
        private async void Test_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            count++;
            tbv.Add("���Ա���", $"�� {count} �α�����");
        }
    }
}