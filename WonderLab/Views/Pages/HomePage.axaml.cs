using Avalonia.Input;
using System.Diagnostics;
using Avalonia.ReactiveUI;
using Avalonia.Interactivity;
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
