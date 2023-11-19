using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
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
    }
}
