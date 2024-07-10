using System.Linq;
using WonderLab.Services;
using WonderLab.Extensions;
using System.Threading.Tasks;
using WonderLab.Services.Game;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WonderLab.Classes.Datas.TaskData;
using WonderLab.Classes.Datas.ViewData;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.ViewModels.Pages;

public sealed partial class HomePageViewModel : ViewModelBase {
    private readonly GameService _gameService;
    private readonly TaskService _taskService;
    private readonly SettingService _settingService;
    private readonly NotificationService _notificationService;

    [ObservableProperty]
    private GameViewData activeGameEntry;

    public bool IsGameEmpty => !GameEntries.Any();
    public ObservableCollection<GameViewData> GameEntries { get; private set; }

    /// <inheritdoc />
    public HomePageViewModel(
        GameService gameService,
        TaskService taskService,
        SettingService settingService,
        NotificationService notificationService) {
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
        var launchTask = new LaunchTask(_gameService, _settingService, _notificationService);
        //launchTask.WaitForRunAsync();
        _taskService.QueueJob(launchTask);
    }
}