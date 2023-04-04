using Avalonia.Media.Imaging;
using Avalonia.Threading;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.Toolkits.Network;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;

namespace wonderlab.Class.ViewData
{
    public class AccountViewData : ViewDataBase<UserModel> {   
        public AccountViewData(UserModel data) : base(data) {
            Dispatcher.UIThread.Post(async () => {
                await GetSkinAsync();
            });
        }

        [Reactive]
        public Bitmap Head { get; set; }

        [Reactive]
        public Bitmap Body { get; set; }

        [Reactive]
        public Bitmap RightHand { get; set; }

        [Reactive]
        public Bitmap LeftHand { get; set; }

        [Reactive]
        public Bitmap RightLeg { get; set; }

        [Reactive]
        public Bitmap LeftLeg { get; set; }

        public async ValueTask GetSkinAsync() {
            try {
                var url = await Task.Run(async () => {
                    return Data.UserType switch {               
                        AccountType.Yggdrasil => await GetYggdrasilSkinUrlAsync(Data.Uuid, Data.YggdrasilUrl),
                        AccountType.Microsoft => await GetMicrosoftSkinUrlAsync(Data.Uuid),
                        _ => string.Empty,
                    };
                });

                byte[] skin = null;
                if(!string.IsNullOrEmpty(url)) {
                    skin = await (await HttpWrapper.HttpGetAsync(url)).Content.ReadAsByteArrayAsync();
                }
                else {
                    var path = Path.Combine(JsonUtils.TempPath, "steve.png");
                    ((Bitmap)BitmapUtils.GetAssetBitmap("steve.png"))!.Save(path);

                    skin = await File.ReadAllBytesAsync(path);
                }

                Head = (await BitmapUtils.CropSkinHeadBitmap(skin)).ToBitmap();
                Body = BitmapUtils.CropSkinBodyBitmap(skin.ToImage()).ToBitmap();
                RightHand = BitmapUtils.CropRightHandBitmap(skin.ToImage()).ToBitmap();
                LeftHand = BitmapUtils.CropLeftHandBitmap(skin.ToImage()).ToBitmap();
                RightLeg = BitmapUtils.CropRightLegBitmap(skin.ToImage()).ToBitmap();
                LeftLeg = BitmapUtils.CropLeftLegBitmap(skin.ToImage()).ToBitmap();
            }
            catch (Exception ex) {
                Trace.WriteLine($"[错误] {ex}");
            }
        }
        
        internal async ValueTask<string> GetMicrosoftSkinUrlAsync(string uuid)
        {
            var res = await HttpWrapper.HttpGetAsync($"https://sessionserver.mojang.com/session/minecraft/profile/{uuid}");
            var json = await res.Content.ReadAsStringAsync();
            Trace.WriteLine($"[信息] 返回的 Json 信息如下：{json}");

            var skinjson = Encoding.UTF8.GetString(Convert.FromBase64String(json.ToJsonEntity<AccountSkinModel>().Properties.First().Value));
            Trace.WriteLine($"[信息] 皮肤 Base64 解码的 Json 信息如下：{skinjson}");

            var url = skinjson.ToJsonEntity<SkinMoreInfo>().Textures.Skin.Url;
            Trace.WriteLine($"[信息] 皮肤的链接如下：{url}");
            return url;
        }

        internal async ValueTask<string> GetYggdrasilSkinUrlAsync(string uuid, string uri){
            var res = await HttpWrapper.HttpGetAsync($"{uri}/sessionserver/session/minecraft/profile/{uuid.Replace("-", string.Empty)}");
            var json = await res.Content.ReadAsStringAsync();
            Trace.WriteLine($"[信息] 返回的 Json 信息如下：{json}");

            var skinjson = Encoding.UTF8.GetString(Convert.FromBase64String(json.ToJsonEntity<AccountSkinModel>().Properties.First().Value));
            Trace.WriteLine($"[信息] 皮肤 Base64 解码的 Json 信息如下：{skinjson}");

            var url = skinjson.ToJsonEntity<SkinMoreInfo>().Textures.Skin.Url;
            Trace.WriteLine($"[信息] 皮肤的链接如下：{url}");
            return url;
        }
    }
}
