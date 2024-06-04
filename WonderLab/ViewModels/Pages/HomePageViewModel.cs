using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Datas.TaskData;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Extensions;
using WonderLab.Services;
using WonderLab.Services.Game;

namespace WonderLab.ViewModels.Pages;

public sealed partial class HomePageViewModel : ViewModelBase {
    private readonly LogService _logService;
    private readonly GameService _gameService;
    private readonly TaskService _taskService;
    private readonly SettingService _settingService;
    private readonly NotificationService _notificationService;

    [ObservableProperty]
    private GameViewData activeGameEntry;
    
    public ObservableCollection<GameViewData> GameEntries { get; private set; }
    
    /// <inheritdoc />
    public HomePageViewModel(
        GameService gameService,
        LogService logService,
        TaskService taskService, 
        SettingService settingService,
        NotificationService notificationService) {
        _logService = logService;
        _gameService = gameService;
        _taskService = taskService;
        _settingService = settingService;
        _notificationService = notificationService;

        GameEntries = _gameService.GameEntries.ToObservableList();

        _ = Task.Run(async () => {
            await Task.Delay(250);
            ActiveGameEntry = _gameService.ActiveGameEntry;
        });
    }

    partial void OnActiveGameEntryChanged(GameViewData value) {
        _gameService.ActivateGameViewEntry(value);
    }

    [RelayCommand]
    private void Launch() {
        _taskService.QueueJob(new LaunchTask(_gameService, _settingService, _notificationService));
    }
}