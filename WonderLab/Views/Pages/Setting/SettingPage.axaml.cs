using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.ViewModels.Pages.Setting;

namespace WonderLab.Views.Pages.Setting;

public partial class SettingPage : UserControl
{
    public SettingPageViewModel ViewModel { get; set; }

    public SettingPage()
    {
        InitializeComponent();
        DataContext = ViewModel = App.ServiceProvider.GetService<SettingPageViewModel>();
    }
}
