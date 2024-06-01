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
    public required (Bitmap head, Bitmap body, Bitmap leftHead, Bitmap rightHead, Bitmap leftLeg, Bitmap rightLeg) Steve { get; init; }

    public SkinService() {
        Steve = GetSteve();
    }

    public ValueTask<byte[]> GetMicrosoftSkinAsync(MicrosoftAccount account) {
        MicrosoftSkinFetcher microsoftSkinFetcher = new(account.Uuid.ToString());
        return microsoftSkinFetcher.GetSkinAsync();
    }

    public ValueTask<byte[]> GetYggdrasilSkinAsync(YggdrasilAccount account) {
        YggdrasilSkinFetcher yggdrasilSkinFetcher = new(account.YggdrasilServerUrl, account.Uuid.ToString());
        return yggdrasilSkinFetcher.GetSkinAsync();
    }

    public (Bitmap head, Bitmap body, Bitmap leftHead, Bitmap rightHead, Bitmap leftLeg, Bitmap rightLeg) GetSkinParts(byte[] skinData) {
        var skinResolver = new SkinResolver(skinData);
        return (skinResolver.CropSkinHeadBitmap().ToBitmap(), 
            skinResolver.CropSkinBodyBitmap().ToBitmap(), 
            skinResolver.CropLeftHandBitmap().ToBitmap(), 
            skinResolver.CropRightHandBitmap().ToBitmap(),
            skinResolver.CropLeftLegBitmap().ToBitmap(), 
            skinResolver.CropRightLegBitmap().ToBitmap());
    }

    private (Bitmap head, Bitmap body, Bitmap leftHead, Bitmap rightHead, Bitmap leftLeg, Bitmap rightLeg) GetSteve() {
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