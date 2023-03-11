using Avalonia.Controls;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class LaunchConfigPage : UserControl
    {
        public static LaunchConfigPageViewModel ViewModel { get; set; } = new();
        public LaunchConfigPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
