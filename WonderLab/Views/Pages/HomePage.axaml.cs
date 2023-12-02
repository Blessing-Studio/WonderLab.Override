using Avalonia.Input;
using Avalonia.ReactiveUI;
using WonderLab.ViewModels.Pages;

namespace WonderLab.Views.Pages {
    public partial class HomePage : ReactiveUserControl<HomePageViewModel> {
        public HomePage(HomePageViewModel vm) {
            InitializeComponent();
            ViewModel = vm;
        }

        public HomePage() {
            InitializeComponent(); 
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e) {
            ViewModel?.CloseGameCoreBar();
        }
    }
}
