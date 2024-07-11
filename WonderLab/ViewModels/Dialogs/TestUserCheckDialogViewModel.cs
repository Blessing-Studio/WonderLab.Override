using Flurl.Http;
using WonderLab.Services;
using WonderLab.Services.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.Notifications;
using WonderLab.Classes.Datas.ViewData;

namespace WonderLab.ViewModels.Dialogs;

public sealed partial class TestUserCheckDialogViewModel : DialogViewModelBase {
    private readonly DialogService _dialogService;
    private readonly WindowService _windowService;
    private readonly SettingService _settingService;
    private readonly NotificationService _notificationService;
    private readonly string _apiKey = "9e15a18a-e726-453b-9004-a670c1dfaca3";

    public TestUserCheckDialogViewModel(NotificationService notificationService, SettingService settingService, DialogService dialogService, WindowService windowService) {
        _dialogService = dialogService;
        _windowService = windowService;
        _settingService = settingService;
        _notificationService = notificationService;
    }

    [RelayCommand]
    private void Close() {
        _windowService.Close();
    }

    [RelayCommand]
    private async Task Authenticate(string uuid) {
        try {
            var result = await "http://47.113.149.130:14514/api/user".WithHeaders(new Dictionary<string, string>() {
                { "x-api-key", _apiKey },
                { "x-user-uuid", uuid },
            }).GetJsonAsync<KeyValuePair<string, string>>();

            _notificationService.QueueJob(new NotificationViewData {
                Title = "你好",
                Content = $"欢迎测试用户 {result.Value} 回来！",
                NotificationType = NotificationType.Success
            });

            _settingService.Data.TestUserUuid = result.Key;
            _dialogService.CloseContentDialog();
        } catch (System.Exception ex) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "错误",
                Content = $"{ex.Message}",
                NotificationType = NotificationType.Error
            });
        }
    }
}