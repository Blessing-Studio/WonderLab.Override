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
using static System.Net.Mime.MediaTypeNames;

namespace wonderlab.Class.Utils
{
    public class GameAccountUtils {
        public static async ValueTask<List<UserModel>> GetUsersAsync() {
            JsonUtils.DirectoryCheck();

            List<UserModel> results = new List<UserModel>();
            var usersFile = Directory.GetFiles(JsonUtils.UserDataPath);
            foreach (var user in usersFile) {
                var text = await File.ReadAllTextAsync(user);
                var data = JsonConvert.DeserializeObject<UserModel>(CryptoToolkit.DecrytOfKaiser(text.ConvertToString()));

                results.Add(data);
            }

            return results;
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
                AccessToken = news.Type is AccountType.Yggdrasil ? (news as YggdrasilAccount).ClientToken : old.AccessToken
            };

            var text = CryptoToolkit.EncrytoOfKaiser(content.ToJson()).ConvertToBase64();
            await File.WriteAllTextAsync(Path.Combine(JsonUtils.UserDataPath, $"{old.Uuid}.dat"), text);
        }
    }
}
