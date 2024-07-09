﻿using System.Threading;
using WonderLab.Services;
using System.Threading.Tasks;
using WonderLab.Services.Game;
using Avalonia.Controls.Notifications;
using WonderLab.Classes.Datas.ViewData;
using MinecraftLaunch.Components.Launcher;
using MinecraftLaunch.Components.Analyzer;
using MinecraftLaunch.Classes.Models.Launch;

namespace WonderLab.Classes.Datas.TaskData;

/// <summary>
/// 游戏启动任务
/// </summary>
public sealed class LaunchTask : TaskBase {
    private readonly GameService _gameService;
    private readonly SettingService _settingService;
    private readonly NotificationService _notificationService;

    public LaunchTask(GameService gameService, 
        SettingService settingService,
        NotificationService notificationService) {
        _gameService = gameService;
        _settingService = settingService;
        _notificationService = notificationService;

        JobName = $"游戏 {_gameService.ActiveGameEntry.Entry.Id} 的启动任务";
    }

    public override async ValueTask BuildWorkItemAsync(CancellationToken token) {
        ReportProgress(0d, "0%");
        _notificationService.QueueJob(new NotificationViewData {
            Title = "信息",
            Content = $"开始启动游戏实例  {_gameService.ActiveGameEntry.Entry.Id}，稍安勿躁！",
            NotificationType = NotificationType.Information
        });

        var data = _settingService.Data;
        var launchConfig = new LaunchConfig() {
            LauncherName = "WonderLab",
            Account = data.ActiveAccount,
            IsEnableIndependencyCore = data.IsGameIndependent,
            JvmConfig = new(data.ActiveJava.JavaPath) {
                MaxMemory = data.MaxMemory,
            },
        };

        var launcher = new Launcher(_gameService.GameResolver, launchConfig);
        var result = await launcher.LaunchAsync(_gameService.ActiveGameEntry.Entry.Id);
        ReportProgress(1d, "100%");
        result.Exited += (_, args) => {
            if (args.ExitCode != 0) {
                _notificationService.QueueJob(new NotificationViewData {
                    Title = "错误",
                    Content = $"游戏实例 {_gameService.ActiveGameEntry.Entry.Id},由于未知异常退出，详细信息请查看日志！",
                    NotificationType = NotificationType.Error
                });

                GameCrashAnalyzer gameCrashAnalyzer = new(_gameService.ActiveGameEntry.Entry,
                    launchConfig.IsEnableIndependencyCore);

                var reports = gameCrashAnalyzer.AnalysisLogs();
                return;
            }

            _notificationService.QueueJob(new NotificationViewData {
                Title = "信息",
                Content = $"游戏实例 {_gameService.ActiveGameEntry.Entry.Id} 已退出",
                NotificationType = NotificationType.Information
            });
        };

        result.OutputLogReceived += (_, args) => {

        };

        _notificationService.QueueJob(new NotificationViewData {
            Title = "成功",
            Content = $"已完成启动步骤，等待游戏进程退出",
            NotificationType = NotificationType.Success
        });

        IsIndeterminate = true;
        await result.Process.WaitForExitAsync(token);
    }
}