using System;
using Flurl.Http;
using System.Threading;
using WonderLab.Services;
using WonderLab.Services.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using WonderLab.ViewModels.Dialogs;
using Avalonia.Controls.Notifications;
using WonderLab.Classes.Datas.ViewData;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Classes.Datas.TaskData;

public sealed class InitTask : TaskBase {
    private readonly DialogService _dialogService;
    private readonly SettingService _settingService;
    private readonly NotificationService _notificationService;
    private readonly string _apiKey = "9e15a18a-e726-453b-9004-a670c1dfaca3";

    public InitTask(SettingService settingService, DialogService dialogService, NotificationService notificationService) {
        IsIndeterminate = true;
        JobName = "程序初始化任务";

        _dialogService = dialogService;
        _settingService = settingService;
        _notificationService = notificationService;
    }

    public async override ValueTask BuildWorkItemAsync(CancellationToken token) {
        await Task.Delay(TimeSpan.FromSeconds(1), token);
        _notificationService.QueueJob(new NotificationViewData {
            Title = "信息",
            Content = $"正在初始化部分服务，稍等片刻",
            NotificationType = NotificationType.Information
        });

        IsIndeterminate = false;

        if (string.IsNullOrEmpty(_settingService.Data.TestUserUuid)) {
            _dialogService.ShowContentDialog<TestUserCheckDialogViewModel>();
            return;
        }

        try {
            var result = await "http://47.113.149.130:14514/api/user".WithHeaders(new Dictionary<string, string>() {
                { "x-api-key", _apiKey },
                { "x-user-uuid", _settingService.Data.TestUserUuid },
            }).GetJsonAsync<KeyValuePair<string, string>>();

            _notificationService.QueueJob(new NotificationViewData {
                Title = "你好",
                Content = $"欢迎测试用户 {result.Value} 回来！",
                NotificationType = NotificationType.Success
            });

            _settingService.Data.TestUserUuid = result.Key;
            if (_dialogService.IsDialogOpen) {
                _dialogService.CloseContentDialog();
            }
        } catch (Exception ex) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "错误",
                Content = $"{ex.Message}",
                NotificationType = NotificationType.Error
            });

            _dialogService.ShowContentDialog<TestUserCheckDialogViewModel>();
        }
    }
}