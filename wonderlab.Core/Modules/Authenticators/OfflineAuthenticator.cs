using MinecraftOAuth.Authenticator;
using MinecraftOAuth.Module.Base;
using MinecraftLaunch.Modules.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MinecraftOAuth.Authenticator {
    /// <summary>
    /// 离线验证器
    /// </summary>
    public partial class OfflineAuthenticator : AuthenticatorBase {
        /// <returns>离线账户实例</returns>
        public override OfflineAccount Auth() => new OfflineAccount {
            AccessToken = Guid.NewGuid().ToString("N"),
            ClientToken = Guid.NewGuid().ToString("N"),
            Name = this.Name,
            Uuid = this.Uuid
        };


        /// <param name="func"></param>
        /// <returns></returns>
        public async ValueTask<OfflineAccount> AuthAsync(Action<string> func = default) => await Task.FromResult(new OfflineAccount {
            AccessToken = Guid.NewGuid().ToString("N"),
            ClientToken = Guid.NewGuid().ToString("N"),
            Name = this.Name,
            Uuid = this.Uuid
        });
    }

    partial class OfflineAuthenticator {
        public OfflineAuthenticator(string name, Guid uuid = default) {
            this.Name = name;
            this.Uuid = uuid;

            if (this.Uuid == default) {
                using var md5 = MD5.Create();
                this.Uuid = new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(this.Name)));
            }
        }

        public string Name { get; set; }

        public Guid Uuid { get; set; }
    }
}
