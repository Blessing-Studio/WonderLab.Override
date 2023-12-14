using Avalonia.Controls;
using Avalonia.Input;
using WonderLab.ViewModels.Pages;

namespace WonderLab.Views.Pages {
    public partial class HomePage : UserControl {
        public HomePageViewModel ViewModel { get; set; }

        public HomePage(HomePageViewModel vm) {
            InitializeComponent();
            DataContext = ViewModel = vm;
        }

        public HomePage() {
            InitializeComponent(); 
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e) {
            ViewModel?.CloseGameCoreBar();
        }
    }
}
