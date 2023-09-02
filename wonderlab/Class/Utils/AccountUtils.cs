using DynamicData;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Utils;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Models;
using wonderlab.Class.ViewData;
using System.Text.Json;

namespace wonderlab.Class.Utils {
    public class AccountUtils {
        public static async IAsyncEnumerable<AccountViewData> GetAsync(bool flag = false) {
            JsonUtils.DirectoryCheck();

            var usersFile = Directory.EnumerateFiles(JsonUtils.UserDataPath);
            foreach (var user in usersFile) {
                var text = await File.ReadAllTextAsync(user);
                yield return JsonSerializer.Deserialize<UserModel>(await Task.Run(() => 
                CryptoUtil.DecrytOfKaiser(text
                .ConvertToString())))!
                .CreateViewData<UserModel, AccountViewData>(flag);
            }
        }

        public static async ValueTask SaveAsync(UserModel user, bool flag = false) {
            var json = user.ToJson();
            var text = CryptoUtil.EncrytoOfKaiser(json).ConvertToBase64();
            await File.WriteAllTextAsync(Path.Combine(JsonUtils.UserDataPath, $"{user.Uuid}.dat"), text);

            if (!CacheResources.Accounts.Any(x=> x.Data.Uuid == user.Uuid && x.Data.UserType == user.UserType)) {
                CacheResources.Accounts.Add(user.CreateViewData<UserModel, AccountViewData>(flag));
            }
        }

        public static async ValueTask RefreshAsync(UserModel old, Account news) {
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

            var text = CryptoUtil.EncrytoOfKaiser(content.ToJson()).ConvertToBase64();
            await File.WriteAllTextAsync(Path.Combine(JsonUtils.UserDataPath, $"{old.Uuid}.dat"), text);
        }

        public static async ValueTask DeleteAsync(UserModel user) {
            await Task.Run(() => {
                File.Delete(Path.Combine(JsonUtils.UserDataPath, $"{user.Uuid}.dat"));
                var index = CacheResources.Accounts.IndexOf(CacheResources.Accounts.Where(x => user.Uuid == x.Data.Uuid).FirstOrDefault()!);
                CacheResources.Accounts.Remove(CacheResources.Accounts[index]);
            });
        }
    }
}
