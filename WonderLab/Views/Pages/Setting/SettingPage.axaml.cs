using Avalonia.Controls;
using WonderLab.ViewModels.Pages.Setting;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Views.Pages.Setting {
    public partial class SettingPage : UserControl {
        public SettingPageViewModel ViewModel { get; set; }

        public SettingPage() {
            InitializeComponent();
            DataContext = ViewModel = App.ServiceProvider.GetService<SettingPageViewModel>();
        }
    }
}
