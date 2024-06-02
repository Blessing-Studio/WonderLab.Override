using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using WonderLab.Services.Auxiliary;
using MinecraftLaunch.Classes.Enums;
using MinecraftLaunch.Classes.Models.Auth;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.Utilities;
using WonderLab.Services;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;

namespace WonderLab.Classes.Datas.ViewData;

public sealed partial class AccountViewData : ObservableObject {
    private readonly SkinService _skinService;
    private readonly SettingService _settingService;

    [ObservableProperty] private Bitmap _head;
    [ObservableProperty] private Bitmap _body;
    [ObservableProperty] private Bitmap _leftLeg;
    [ObservableProperty] private Bitmap _rightLeg;
    [ObservableProperty] private Bitmap _leftHand;
    [ObservableProperty] private Bitmap _rightHand;

    [ObservableProperty] private string _uuid;

    public Account Account { get; init; }

    public AccountViewData(Account account) {
        Account = account;
        _skinService = App.ServiceProvider.GetService<SkinService>();
        _settingService = App.ServiceProvider.GetService<SettingService>();

        _ = InitAsync();
    }

    [RelayCommand]
    private void Delete() {
        if (_settingService.Data.Accounts.Remove(Account)) {
            WeakReferenceMessenger.Default.Send(new AccountChangeNotificationMessage(this));
        }
    }

    private async Task InitAsync() {
        (Bitmap head, Bitmap body, Bitmap leftHead, Bitmap rightHead, Bitmap leftLeg, Bitmap rightLeg) skinParts = default;
        await Task.Run(async () => {
            if (Account.Type is AccountType.Offline) {
                skinParts = _skinService.Steve;
            } else {
                var skinData = Account.Type switch {
                    AccountType.Microsoft => _skinService.GetMicrosoftSkinAsync(Account as MicrosoftAccount),
                    AccountType.Yggdrasil => _skinService.GetYggdrasilSkinAsync(Account as YggdrasilAccount),
                    _ => default
                };
                
                var result = await skinData;
                if (result is null) {
                    skinParts = _skinService.Steve;
                    return;
                }

                skinParts = _skinService.GetSkinParts(result);
            }
        });

        Head = skinParts.head;
        Body = skinParts.body;
        LeftLeg = skinParts.leftLeg;
        LeftHand = skinParts.leftHead;
        RightLeg = skinParts.rightLeg;
        RightHand = skinParts.rightHead;
        Uuid = StringUtil.ReplaceUuid(Account.Uuid.ToString());
    }
}