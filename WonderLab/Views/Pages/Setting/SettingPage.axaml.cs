using Avalonia.Controls;
using WonderLab.ViewModels.Pages.Setting;

namespace WonderLab.Views.Pages.Setting {
    public partial class SettingPage : UserControl {
        public SettingPageViewModel ViewModel { get; set; }

        public SettingPage(SettingPageViewModel vm) {
            InitializeComponent();
            DataContext = ViewModel = vm;
        }

        public SettingPage() {
            InitializeComponent();
        }
    }
}
