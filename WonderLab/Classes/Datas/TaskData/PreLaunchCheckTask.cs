using System.Threading;
using System.Threading.Tasks;
using WonderLab.Services;
using WonderLab.Services.UI;
using WonderLab.Services.Game;
using System.Linq;
using WonderLab.Classes.Datas.ViewData;
using MinecraftLaunch.Components.Fetcher;
using System.Collections.Immutable;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Components.Checker;
using Avalonia.Controls.Notifications;
using MinecraftLaunch.Extensions;
using MinecraftLaunch;
using WonderLab.Services.Download;
using MinecraftLaunch.Classes.Enums;
using WonderLab.Services.Auxiliary;
using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Classes.Models.Auth;
using System;
using WonderLab.ViewModels.Dialogs.Setting;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;

namespace WonderLab.Classes.Datas.TaskData;

public sealed class PreLaunchCheckTask : TaskBase {
    private readonly JavaFetcher _javaFetcher;
    private readonly ResourceChecker _resourceChecker;
    private readonly WeakReferenceMessenger _weakReferenceMessenger;

    private readonly GameService _gameService;
    private readonly DialogService _dialogService;
    private readonly AccountService _accountService;
    private readonly SettingService _settingService;
    private readonly DownloadService _downloadService;
    private readonly NotificationService _notificationService;

    private bool _isReturnTrue;
    private ImmutableArray<JavaEntry> _javas;
    private CancellationTokenSource _accountRefreshCancellationToken;

    public event EventHandler<bool> CanLaunch;

    public CancellationTokenSource CheckTaskCancellationToken { get; private set; }

    public PreLaunchCheckTask(
        JavaFetcher javaFetcher,
        GameService gameService,
        DialogService dialogService,
        SettingService settingService,
        AccountService accountService,
        DownloadService downloadService,
        NotificationService notificationService,
        WeakReferenceMessenger weakReferenceMessenger) {
        _gameService = gameService;
        _dialogService = dialogService;
        _settingService = settingService;
        _accountService = accountService;
        _downloadService = downloadService;
        _notificationService = notificationService;

        _javaFetcher = javaFetcher;
        _weakReferenceMessenger = weakReferenceMessenger;
        _resourceChecker = new(_gameService.ActiveGameEntry.Entry);

        JobName = "预启动检查任务";
        IsIndeterminate = true;
        CheckTaskCancellationToken = new();
        _accountRefreshCancellationToken = new();

        _weakReferenceMessenger.Register<RefreshAccountFinishMessage>(this, (_, args) => {
            _accountRefreshCancellationToken.Cancel();
            _accountRefreshCancellationToken.Dispose();
        });
    }

    public override async ValueTask BuildWorkItemAsync(CancellationToken token) {
        try {
            _isReturnTrue = true;
            await Task.Run(CheckJavaAndExecute, token);
            await Task.Run(CheckResourcesAndExecute, token);
            await Task.Run(CheckAccountAndExecute, token);

            IsIndeterminate = false;
            ReportProgress(1, "预启动检查完成");
            CanLaunch?.Invoke(this, _isReturnTrue);
        } catch (Exception) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "错误",
                Content = "预启动检查失败，原因：遭遇了未知错误！",
                NotificationType = NotificationType.Error
            });

            await CheckTaskCancellationToken.CancelAsync();
            return;
        }
    }

    async Task CheckJavaAndExecute() {
        ReportProgress("正在检查 Java 相关信息");
        var resultJava = await CheckJavaAsync();
        if (!resultJava.value && !resultJava.canExecute) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "错误",
                Content = "未找到 Java，请先添加一个 Java 后再尝试启动游戏!",
                NotificationType = NotificationType.Error
            });

            _isReturnTrue = false;
            CanBeCancelled = true;
            await CheckTaskCancellationToken.CancelAsync();
        } else if (!resultJava.value && resultJava.canExecute) {
            _settingService.Data.Javas = [.. _javas];
            _settingService.Data.ActiveJava = _javas.FirstOrDefault();
        }
    }

    async Task CheckResourcesAndExecute() {
        ReportProgress("正在检查游戏本体资源完整性");
        var resultResource = await CheckResourcesAsync();
        if (!resultResource) {
            IsIndeterminate = false;
            _notificationService.QueueJob(new NotificationViewData {
                Title = "警告",
                Content = $"发现了 {_resourceChecker.MissingResources.Count} 个缺失资源，正在尝试将其补全！",
                NotificationType = NotificationType.Warning
            });

            var downloadSource = _settingService.Data.IsUseMirrorDownloadSource
                ? MirrorDownloadManager.Bmcl
                : null;
            
            var resultComplete = await _resourceChecker.MissingResources.DownloadResourceEntrysAsync(downloadSource, args => {
                var percentage = args.ToPercentage() * 100;
                ReportProgress(percentage, $"{args.CompletedCount}/{args.TotalCount} - {percentage:0.00}%");
            }, _downloadService.MainDownloadRequest);

            if (!resultComplete) {
                _notificationService.QueueJob(new NotificationViewData {
                    Title = "警告",
                    Content = "有一个或多个资源下载失败，WonderLab 会继续尝试启动游戏，但是大概率会出现问题！",
                    NotificationType = NotificationType.Warning
                });
            }

            ReportProgress(0);
        }
    }

    async Task CheckAccountAndExecute() {
        IsIndeterminate = true;
        ReportProgress("正在验证账户信息");
        var (value, canExecute) = await CheckAccountAsync();
        if (!value && canExecute) {
            ReportProgress("账户信息过期，正在执行刷新");
            _dialogService.ShowContentDialog<RefreshAccountDialogViewModel>(_settingService.Data.ActiveAccount);
            await WaitForRunAsync(_accountRefreshCancellationToken.Token);
        } else if (!value && !canExecute) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "错误",
                Content = "未找到账户信息，请先添加一个账户后再尝试启动游戏！",
                NotificationType = NotificationType.Error
            });

            CanBeCancelled = true;
            _isReturnTrue = false;
            await CheckTaskCancellationToken.CancelAsync();
            return;
        }
    }

    private async ValueTask<(bool value, bool canExecute)> CheckJavaAsync() {
        _javas = await _javaFetcher.FetchAsync();
        var data = _settingService.Data;
        if (data is null) {
            return (false, !_javas.IsEmpty);
        }

        return data.IsAutoSelectJava
            ? (data.Javas?.Count != 0, !_javas.IsEmpty)
            : (data.ActiveJava is not null, !_javas.IsEmpty);
    }

    private async ValueTask<bool> CheckResourcesAsync() {
        return await _resourceChecker.CheckAsync();
    }

    private async ValueTask<(bool value, bool canExecute)> CheckAccountAsync() {
        var activeAccount = _settingService.Data.ActiveAccount;

        if (activeAccount is null) {
            return (false, false);
        }

        try {
            switch (activeAccount.Type) {
                case AccountType.Offline:
                    return (true, false);
                case AccountType.Yggdrasil:
                    _accountService.InitializeComponent(new YggdrasilAuthenticator(activeAccount as YggdrasilAccount), AccountType.Yggdrasil);
                    _settingService.Data.ActiveAccount = (await _accountService.AuthenticateAsync(3)).FirstOrDefault();
                    break;
                case AccountType.Microsoft:
                    _accountService.InitializeComponent(new MicrosoftAuthenticator(activeAccount as MicrosoftAccount,
                        "9fd44410-8ed7-4eb3-a160-9f1cc62c824c", true),
                        AccountType.Microsoft,
                        true);

                    _settingService.Data.ActiveAccount = (await _accountService.AuthenticateAsync(2)).FirstOrDefault();
                    break;
            }

            return (false, false);
        } catch (Exception) {
            return (false, true);
        }
    }
}