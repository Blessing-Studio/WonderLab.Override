using Avalonia.Controls;
using WonderLab.ViewModels.Pages.Setting;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Views.Pages.Setting {
    public partial class LaunchSettingPage : UserControl {
        public LaunchSettingPageViewModel ViewModel { get; set; }

        public LaunchSettingPage() {
            InitializeComponent();
            DataContext = ViewModel = App.ServiceProvider.GetService<LaunchSettingPageViewModel>()!;
        }
    }
}
