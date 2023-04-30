using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.Toolkits.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.ViewData;
using static System.Net.Mime.MediaTypeNames;

namespace wonderlab.Class.Utils
{
    public static class GameAccountUtils {
        public static async IAsyncEnumerable<AccountViewData> GetUsersAsync() {
            JsonUtils.DirectoryCheck();

            var usersFile = Directory.EnumerateFiles(JsonUtils.UserDataPath);
            foreach (var user in usersFile) {
                var text = await File.ReadAllTextAsync(user);
                yield return JsonConvert.DeserializeObject<UserModel>(await Task.Run(() => CryptoToolkit.DecrytOfKaiser(text.ConvertToString())))!.CreateViewData<UserModel, AccountViewData>();
            }
        }

        public static async ValueTask SaveUserDataAsync(UserModel user) { 
            var json = user.ToJson(true);
            var text = CryptoToolkit.EncrytoOfKaiser(json).ConvertToBase64();
            await File.WriteAllTextAsync(Path.Combine(JsonUtils.UserDataPath,$"{user.Uuid}.dat"), text);
        }

        public static async ValueTask RefreshUserDataAsync(UserModel old, Account news) {
            var content = new UserModel() {
                UserName = news.Name,
                Uuid = news.Uuid.ToString(),
                UserToken = news.AccessToken,
                UserType = news.Type,
                Email = old.Email,
                Password = old.Password,
                YggdrasilUrl = old.YggdrasilUrl,
                AccessToken = news.Type is AccountType.Yggdrasil ? (news as YggdrasilAccount)!.ClientToken : old.AccessToken
            };

            var text = CryptoToolkit.EncrytoOfKaiser(content.ToJson()).ConvertToBase64();
            await File.WriteAllTextAsync(Path.Combine(JsonUtils.UserDataPath, $"{old.Uuid}.dat"), text);
        }
    }
}
