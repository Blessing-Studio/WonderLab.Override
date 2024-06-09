using System.Threading.Tasks;
using MinecraftLaunch.Utilities;
using WonderLab.Classes.Interfaces;
using WonderLab.Services.Navigation;
using WonderLab.ViewModels.Pages.Oobe;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.ViewModels.Windows;

public sealed partial class OobeWindowViewModel : ViewModelBase {
    private readonly INavigationService _navigationService;

    [ObservableProperty] private object _activePage;
    [ObservableProperty] private bool _isTitleBarVisible;
    [ObservableProperty] private bool _isOpenBackgroundPanel;

    public OobeWindowViewModel(OobeNavigationService navigationService) {
        IsTitleBarVisible = EnvironmentUtil.IsWindow;
        Task.Run(async () => {
            await Task.Delay(800);
            IsOpenBackgroundPanel = true;
        });

        _navigationService = navigationService;
        _navigationService.NavigationRequest += p => {
            ActivePage = p.Page;
        };

        _navigationService.NavigationTo<OobeWelcomePageViewModel>();
    }
}