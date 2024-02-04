using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.ViewModels.Pages.Setting;

namespace WonderLab.Views.Pages.Setting;

public partial class LaunchSettingPage : UserControl
{
    public LaunchSettingPageViewModel ViewModel { get; set; }

    public LaunchSettingPage()
    {
        InitializeComponent();
        DataContext = ViewModel = App.ServiceProvider.GetService<LaunchSettingPageViewModel>()!;
    }
}
