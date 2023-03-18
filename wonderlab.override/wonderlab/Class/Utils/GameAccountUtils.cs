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

namespace wonderlab.Class.Utils
{
    public class GameAccountUtils {
        public static async ValueTask<List<UserModel>> GetUsersAsync() {
            JsonUtils.DirectoryCheck();

            List<UserModel> results = new List<UserModel>();
            var usersFile = Directory.GetFiles(JsonUtils.UserDataPath);
            foreach (var user in usersFile) {
                var text = await File.ReadAllTextAsync(user);
                var data  = JsonConvert.DeserializeObject<UserModel>(CryptoToolkit.DecrytOfKaiser(text.ConvertToString()));

                results.Add(data);
            }

            return results;
        }

        public static async ValueTask SaveUserDataAsync(UserModel user) { 
            var json = user.ToJson();
            var text = CryptoToolkit.EncrytoOfKaiser(json).ConvertToBase64();
            await File.WriteAllTextAsync(Path.Combine(JsonUtils.UserDataPath,$"{user.UserName}.dat"), text);
        }
    }
}
