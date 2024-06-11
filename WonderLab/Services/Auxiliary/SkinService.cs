using System;
using System.IO;
using Avalonia.Platform;
using WonderLab.Extensions;
using MinecraftLaunch.Skin;
using Avalonia.Media.Imaging;
using System.Threading.Tasks;
using MinecraftLaunch.Skin.Class.Fetchers;
using MinecraftLaunch.Classes.Models.Auth;

namespace WonderLab.Services.Auxiliary;

/// <summary>
/// 皮肤服务类
/// </summary>
public sealed class SkinService {
    private readonly LogService _logService;
    public required (Bitmap head, Bitmap body, Bitmap leftHead, Bitmap rightHead, Bitmap leftLeg, Bitmap rightLeg) Steve { get; init; }

    public SkinService(LogService logService) {
        _logService = logService;
        Steve = GetSteve();
    }

    public async Task<byte[]> GetMicrosoftSkinAsync(MicrosoftAccount account) {
        _logService.Info(nameof(SkinService), $"开始为 {account.Type} 账户 {account.Name} 获取 Skin");
        MicrosoftSkinFetcher microsoftSkinFetcher = new(account.Uuid.ToString());
        return await microsoftSkinFetcher.GetSkinAsync();
    }

    public async Task<byte[]> GetYggdrasilSkinAsync(YggdrasilAccount account) {
        _logService.Info(nameof(SkinService), $"开始为 {account.Type} 账户 {account.Name} 获取 Skin");
        YggdrasilSkinFetcher yggdrasilSkinFetcher = new(account.YggdrasilServerUrl, account.Uuid.ToString());
        return await yggdrasilSkinFetcher.GetSkinAsync();
    }

    public (Bitmap head, Bitmap body, Bitmap leftHead, Bitmap rightHead, Bitmap leftLeg, Bitmap rightLeg) GetSkinParts(byte[] skinData) {
        var skinResolver = new SkinResolver(skinData);
        _logService.Info(nameof(SkinService), $"开始肢解 Skin 部位");

        return (skinResolver.CropSkinHeadBitmap().ToBitmap(), 
            skinResolver.CropSkinBodyBitmap().ToBitmap(), 
            skinResolver.CropLeftHandBitmap().ToBitmap(), 
            skinResolver.CropRightHandBitmap().ToBitmap(),
            skinResolver.CropLeftLegBitmap().ToBitmap(), 
            skinResolver.CropRightLegBitmap().ToBitmap());
    }

    private (Bitmap head, Bitmap body, Bitmap leftHead, Bitmap rightHead, Bitmap leftLeg, Bitmap rightLeg) GetSteve() {
        _logService.Info(nameof(SkinService), $"开始获取默认 Skin");

        var memoryStream = new MemoryStream();
        using var stream = AssetLoader.Open(new Uri($"resm:WonderLab.Assets.Images.steve.png"));
        stream!.CopyTo(memoryStream);
        memoryStream.Position = 0;

        string imagePath = Path.Combine(Path.GetTempPath(), "steve.png");
        if (!File.Exists(imagePath)) {
            using var bitmap = new Bitmap(memoryStream);
            bitmap.Save(imagePath);
        }

        return GetSkinParts(File.ReadAllBytes(imagePath));
    }
}