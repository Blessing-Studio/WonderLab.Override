using MinecraftOAuth.Module.Base;
using MinecraftOAuth.Module.Models;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;
using Flurl.Http;

namespace MinecraftOAuth.Authenticator {
    /// <summary>
    /// 第三方验证器
    /// </summary>
    public partial class YggdrasilAuthenticator : AuthenticatorBase {
        public string? Uri { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? ClientToken { get; private set; }

        public YggdrasilAuthenticator() { }

        /// <summary>
        /// 标准第三方验证器构造方法
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public YggdrasilAuthenticator(string uri, string email, string password) {
            Uri = uri;
            Email = email;
            Password = password;
        }

        /// <summary>
        /// LittleSkin验证器接口
        /// </summary>
        /// <param name="IsLittleSkin"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public YggdrasilAuthenticator(bool IsLittleSkin, string email, string password) {
            if (IsLittleSkin)
                Uri = "https://littleskin.cn/api/yggdrasil";
            Email = email;
            Password = password;
        }

        /// <summary>
        /// Mojang验证构造器（已弃用）
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>                
        [Obsolete] public YggdrasilAuthenticator(string email, string password) { }

        /// <summary>
        /// 身份验证方法
        /// </summary>
        /// <returns></returns>
        public new IEnumerable<YggdrasilAccount> Auth() => AuthAsync().Result;

        /// <summary>
        /// 异步身份验证方法 
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public async ValueTask<IEnumerable<YggdrasilAccount>> AuthAsync() {
            string content = string.Empty;
            var requestJson = new {
                clientToken = Guid.NewGuid().ToString("N"),
                username = Email,
                password = Password,
                requestUser = false,
                agent = new {
                    name = "Minecraft",
                    version = 1
                }
            };

            List<YggdrasilAccount> accounts = new();
            var result = await $"{Uri}{(string.IsNullOrEmpty(Uri) ? "https://authserver.mojang.com" : "/authserver")}/authenticate"
                .PostJsonAsync(requestJson);

            content = await result.GetStringAsync();
            var accountMessage = content.ToJsonEntity<YggdrasilResponse>();

            ClientToken = accountMessage.ClientToken;
            foreach (var i in accountMessage.UserAccounts) {
                accounts.Add(new YggdrasilAccount() {
                    AccessToken = accountMessage.AccessToken,
                    ClientToken = accountMessage.ClientToken,
                    Name = i.Name,
                    Uuid = Guid.Parse(i.Uuid),
                    YggdrasilServerUrl = this.Uri!,
                    Email = this.Email!,
                    Password = this.Password!
                });
            }

            return accounts;
        }

        /// <summary>
        /// 异步刷新验证方法
        /// </summary>
        /// <returns></returns>
        public async ValueTask<YggdrasilAccount> RefreshAsync(YggdrasilAccount selectProfile) {
            var content = new {
                clientToken = selectProfile.ClientToken,
                accessToken = selectProfile.AccessToken,
                requestUser = true
            };

            using var refreshResult = await $"{Uri}{(string.IsNullOrEmpty(Uri) ? "https://authserver.mojang.com" : "/authserver")}/refresh"
                .PostJsonAsync(content);

            string result = await refreshResult.GetStringAsync();
            refreshResult.ResponseMessage.EnsureSuccessStatusCode();
            var responses = result.ToJsonEntity<YggdrasilResponse>().UserAccounts;

            foreach (var i in responses) {
                if (i.Uuid.Equals(selectProfile.Uuid.ToString().Replace("-", ""))) {
                    return new() {
                        AccessToken = result.ToJsonEntity<YggdrasilResponse>().AccessToken,
                        ClientToken = result.ToJsonEntity<YggdrasilResponse>().ClientToken,
                        Name = i.Name,
                        Uuid = Guid.Parse(i.Uuid),
                        YggdrasilServerUrl = this.Uri!,
                        Email = this.Email!,
                        Password = this.Password!
                    };
                }
            }

            return null!;//执行到此处的原因可能是此角色已删除的原因导致的
        }

        /// <summary>
        /// 异步登出方法
        /// </summary>
        /// <returns></returns>
        public async ValueTask<bool> SignoutAsync() {
            string content = JsonSerializer.Serialize(new {
                    username = Email,
                    password = Password
                }
            );

            using var result = await $"{Uri}{(string.IsNullOrEmpty(Uri) ? "https://authserver.mojang.com" : "/authserver")}/signout"
                .PostJsonAsync(content);

            return result.ResponseMessage.IsSuccessStatusCode;
        }

        /// <summary>
        /// 异步验证方法
        /// </summary>
        /// <param name="accesstoken"></param>
        /// <returns></returns>
        public async ValueTask<bool> ValidateAsync(string accesstoken) {
            var content = new {
                clientToken = ClientToken,
                accesstoken = accesstoken
            };

            using var result = await $"{Uri}/authserver/validate".PostJsonAsync(content);
            return result.ResponseMessage.IsSuccessStatusCode;
        }
    }
}