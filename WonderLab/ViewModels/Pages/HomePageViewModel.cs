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
using MinecraftLaunch.Components.Fetcher;
using WonderLab.Services.UI;
using WonderLab.Services.Auxiliary;
using WonderLab.Services.Download;
using CommunityToolkit.Mvvm.Messaging;
using Avalonia.Controls.Notifications;

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
        if (_gameService.ActiveGameEntry is null) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "错误",
                Content = "无法启动，原因：未选择任何游戏实例！",
                NotificationType = NotificationType.Error
            });

            return;
        }

        var preCheckTask = new PreLaunchCheckTask(App.GetService<JavaFetcher>(),
            _gameService,
            App.GetService<DialogService>(), 
            _settingService, App.GetService<AccountService>(),
            App.GetService<DownloadService>(),
            _notificationService,
            App.GetService<WeakReferenceMessenger>());

        preCheckTask.CanLaunch += (_, arg) => {
            if (arg) {
                var launchTask = new LaunchTask(_gameService, _settingService, _notificationService);
                _taskService.QueueJob(launchTask);
            }
        };

        _taskService.QueueJob(preCheckTask);
    }
}