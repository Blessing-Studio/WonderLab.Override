using WonderLab.Services.UI;
using System.Threading.Tasks;
using WonderLab.Services.Wrap;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using WonderLab.Services;
using WonderLab.Classes.Datas.ViewData;
using Avalonia.Controls.Notifications;

namespace WonderLab.ViewModels.Dialogs.Multiplayer;

public sealed partial class CreateMutilplayerDialogViewModel  : DialogViewModelBase {
    private readonly WrapService _wrapService;
    private readonly DialogService _dialogService;
    private readonly NotificationService _notificationService;
    private readonly ILogger<CreateMutilplayerDialogViewModel> _logger;

    [ObservableProperty] private bool _isConnecting;

    public CreateMutilplayerDialogViewModel(
        WrapService wrapService, 
        DialogService dialogService, 
        NotificationService notificationService,
        ILogger<CreateMutilplayerDialogViewModel> logger) {
        _logger = logger;
        _wrapService = wrapService;
        _dialogService = dialogService;
        _notificationService = notificationService;
    }

    [RelayCommand]
    private async Task Create() {
        IsConnecting = true;
    }
}