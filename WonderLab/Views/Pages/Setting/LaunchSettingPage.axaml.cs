using Avalonia.Controls;
using WonderLab.ViewModels.Pages.Setting;

namespace WonderLab.Views.Pages.Setting {
    public partial class LaunchSettingPage : UserControl {
        public LaunchSettingPageViewModel ViewModel { get; set; }

        public LaunchSettingPage(LaunchSettingPageViewModel vm) {
            InitializeComponent();
            DataContext = ViewModel = vm;        
        }

        public LaunchSettingPage() {
            InitializeComponent();
        }
    }
}
