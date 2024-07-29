using System.Threading.Tasks;
using MinecraftLaunch.Utilities;
using WonderLab.Classes.Interfaces;
using WonderLab.Services.Navigation;
using WonderLab.ViewModels.Pages.Oobe;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;

namespace WonderLab.ViewModels.Windows;

public sealed partial class OobeWindowViewModel : ViewModelBase {
    private readonly INavigationService _navigationService;

    [ObservableProperty] private object _activePage;
    [ObservableProperty] private int _currentPageId;
    [ObservableProperty] private bool _isTitleBarVisible;
    [ObservableProperty] private bool _isOpenBackgroundPanel;
    
    public OobeWindowViewModel(OobeNavigationService navigationService) {
        IsTitleBarVisible = EnvironmentUtil.IsWindow;
        RunBackgroundWork(async () => {
            await Task.Delay(800);
            IsOpenBackgroundPanel = true;
        });

        _navigationService = navigationService;
        _navigationService.NavigationRequest += p => {
            ActivePage = p.Page;
        };

        CurrentPageId = 0;
        _navigationService.NavigationTo<OobeWelcomePageViewModel>();
        WeakReferenceMessenger.Default.Register<OobePageMessage>(this, Handle);
    }

    private void Handle(object sender, OobePageMessage message) {
        switch (message.PageKey) {
            case "OOBELanguage":
                CurrentPageId = 1;
                _navigationService.NavigationTo<OobeLanguagePageViewModel>();
                break;
            case "OOBEAccount":
                CurrentPageId = 2;
                _navigationService.NavigationTo<OobeAccountPageViewModel>();
                break;
        }
    }
}