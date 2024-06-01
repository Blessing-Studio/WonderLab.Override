using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using WonderLab.Services.Auxiliary;
using MinecraftLaunch.Classes.Enums;
using MinecraftLaunch.Classes.Models.Auth;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.Utilities;

namespace WonderLab.Classes.Datas.ViewData;

public sealed partial class AccountViewData : ObservableObject {
    private readonly SkinService _skinService;

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

        _ = InitAsync();
    }

    private async ValueTask InitAsync() {
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

                skinParts = _skinService.GetSkinParts(await skinData);
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