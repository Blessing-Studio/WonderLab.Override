using Avalonia.Controls;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages {
    public partial class ConsoleCenterPage : UserControl {
        public static ConsoleCenterPageViewModel  ViewModel { get; set; }
        public ConsoleCenterPage() {
            InitializeComponent();
            DataContext = ViewModel = new();
        }
    }
}
