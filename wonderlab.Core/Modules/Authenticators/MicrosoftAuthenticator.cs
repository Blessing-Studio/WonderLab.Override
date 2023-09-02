using System.Diagnostics;
using System.Net;
using MinecraftOAuth.Module.Base;
using MinecraftOAuth.Module.Enum;
using MinecraftOAuth.Module.Models;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Utils;
using Flurl.Http;
using System.Text.Json.Nodes;

namespace MinecraftOAuth.Authenticator {
    /// <summary>
    /// 微软验证器
    /// </summary>
    public partial class MicrosoftAuthenticator : AuthenticatorBase {
        /// <summary>
        /// 获取一次性验证代码
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public async ValueTask<DeviceCodeResponse> GetDeviceInfo() {
            if (string.IsNullOrEmpty(ClientId))
                throw new ArgumentNullException("ClientId为空！");

            //开始获取一次性验证代码
            using (var client = new HttpClient()) {
                string tenant = "/consumers";
                var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                    ["client_id"] = ClientId,
                    ["tenant"] = tenant,
                    ["scope"] = string.Join(" ", Scopes)
                });

                var req = new HttpRequestMessage(HttpMethod.Post, "https://login.microsoftonline.com/consumers/oauth2/v2.0/devicecode");
                req.Content = content;
                var res = await client.SendAsync(req);

                string json = await res.Content.ReadAsStringAsync();
                var codeResponse = json.ToJsonEntity<DeviceCodeResponse>();
                return codeResponse;
            }
        }

        /// <summary>
        /// 轮询获取令牌信息
        /// </summary>
        /// <returns></returns>
        public async ValueTask<TokenResponse> GetTokenResponse(DeviceCodeResponse codeResponse) {
            using (HttpClient client = new()) {
                //开始轮询
                string tenant = "/consumers";
                TimeSpan pollingInterval = TimeSpan.FromSeconds(codeResponse.Interval);
                DateTimeOffset codeExpiresOn = DateTimeOffset.UtcNow.AddSeconds(codeResponse.ExpiresIn);
                TimeSpan timeRemaining = codeExpiresOn - DateTimeOffset.UtcNow;
                TokenResponse tokenResponse = default!;

                while (timeRemaining.TotalSeconds > 0) {
                    var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://login.microsoftonline.com/consumers/oauth2/v2.0/token") {
                        Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                            ["grant_type"] = "urn:ietf:params:oauth:grant-type:device_code",
                            ["device_code"] = codeResponse.DeviceCode,
                            ["client_id"] = ClientId,
                            ["tenant"] = tenant
                        })
                    };

                    var tokenRes = await client.SendAsync(tokenRequest);

                    string tokenJson = await tokenRes.Content.ReadAsStringAsync();

                    var tempTokenResponse = JsonNode.Parse(tokenJson);

                    if (tokenRes.StatusCode == HttpStatusCode.OK) {
                        tokenResponse = new() {
                            AccessToken = tempTokenResponse!["access_token"]!.GetValue<string>(),
                            RefreshToken = tempTokenResponse["refresh_token"]!.GetValue<string>(),
                            ExpiresIn = tempTokenResponse["expires_in"]!.GetValue<int>(),
                        };
                    }
                   
                    if (tempTokenResponse["token_type"]?.GetValue<string>() is "Bearer") {
                        AccessToken = tokenResponse.AccessToken;
                        RefreshToken = tokenResponse.RefreshToken;
                        return tokenResponse;
                    }

                    await Task.Delay(pollingInterval);
                    timeRemaining = codeExpiresOn - DateTimeOffset.UtcNow;
                }

                throw new TimeoutException("登录操作已超时");
            }
        }

        public new async ValueTask<MicrosoftAccount> AuthAsync(Action<string> func) {
            #region Authenticate with Refresh

            if (AuthType is AuthType.Refresh) {
                func("开始获取 AccessToken");
                var url = "https://login.live.com/oauth20_token.srf";

                var content = new {
                    client_id = ClientId,
                    refresh_token = RefreshToken,
                    grant_type = "refresh_token",
                };

                var result = await url.PostUrlEncodedAsync(content);
                var tokenResponse = (await result.GetStringAsync()).ToJsonEntity<TokenResponse>();
                AccessToken = (tokenResponse?.AccessToken) ?? "None";
            }

            #endregion

            #region Authenticate with XBL

            func("开始获取 XBL 令牌");

            var rpsTicket = $"d={AccessToken}";
            var xblContent = new {
                Properties = new {
                    AuthMethod = "RPS",
                    SiteName = "user.auth.xboxlive.com",
                    RpsTicket = rpsTicket
                },
                RelyingParty = "http://auth.xboxlive.com",
                TokenType = "JWT"
            };
            
            using var xBLReqModelPostRes = await $"https://user.auth.xboxlive.com/user/authenticate"
                .PostJsonAsync(xblContent);

            var xBLResModel = (await xBLReqModelPostRes.GetStringAsync())
                .ToJsonEntity<XBLAuthenticateResponseModel>();

            #endregion

            #region Authenticate with XSTS

            func("开始获取 XSTS令牌");

            var xstsContent = new {
                Properties = new {
                    SandboxId = "RETAIL",
                    UserTokens = new[] {                   
                        xBLResModel.Token
                    }
                },
                RelyingParty = "rp://api.minecraftservices.com/",
                TokenType = "JWT"
            };


            using var xSTSReqModelPostRes = await $"https://xsts.auth.xboxlive.com/xsts/authorize".PostJsonAsync(xstsContent);
            var xSTSResModel = (await xSTSReqModelPostRes.GetStringAsync()).ToJsonEntity<XSTSAuthenticateResponseModel>();

            #endregion

            #region Authenticate with Minecraft

            func("开始获取 Minecraft账户基础信息");
            var authenticateMinecraftPost = new {
                identityToken = $"XBL3.0 x={xBLResModel.DisplayClaims.Xui[0]
                .GetProperty("uhs")
                .GetString()};{xSTSResModel!.Token}"
            };

            using var authenticateMinecraftPostRes = await $"https://api.minecraftservices.com/authentication/login_with_xbox"
                .PostJsonAsync(authenticateMinecraftPost);

            string access_token = JsonNode.Parse(await authenticateMinecraftPostRes.GetStringAsync())!["access_token"]!
                .GetValue<string>();

            #endregion

            #region Check with Game
            func("开始检查游戏所有权");
            bool hasGame = false;
            try {
                using var gameHasRes = await "https://api.minecraftservices.com/entitlements/mcstore"
                    .WithHeader("Authorization", $"Bearer {access_token}")
                    .GetAsync();

                string json = await gameHasRes.GetStringAsync();
                var itemArray = json.ToJsonEntity<GameHasCheckResponseModel>();

                if (itemArray.Items != null) {
                    hasGame = itemArray.Items.Count > 0 ? true : false;
                } else {
                    hasGame = true;
                }
            }
            catch (Exception ex) {
                Trace.WriteLine($"[错误] 未能成功检查游戏所有权，但会继续尝试获取 个人档案，错误的原因如下：{ex}");
            }
            #endregion

            #region Get the profile

            if (hasGame) {
                func("开始获取 玩家Profile");
                using var profileRes = await "https://api.minecraftservices.com/minecraft/profile"
                    .WithHeader("Authorization", $"Bearer {access_token}")
                    .GetAsync();

                var microsoftAuthenticationResponse = (await profileRes.GetStringAsync())
                    .ToJsonEntity<MicrosoftAuthenticationResponse>();

                func("微软登录（非刷新验证）完成");

                return new MicrosoftAccount {
                    AccessToken = access_token,
                    Type = AccountType.Microsoft,
                    ClientToken = Guid.NewGuid().ToString("N"),
                    Name = microsoftAuthenticationResponse!.Name,
                    Uuid = Guid.Parse(microsoftAuthenticationResponse.Id),
                    RefreshToken = string.IsNullOrEmpty(RefreshToken) ? "None" : RefreshToken,
                    DateTime = DateTime.Now
                };
            } else {
                throw new("未购买 Minecraft！");
            }

            throw new("验证失败！");
            #endregion
        }

        public override MicrosoftAccount Auth() => AuthAsync(null).GetAwaiter().GetResult();
    }

    partial class MicrosoftAuthenticator {
        public MicrosoftAuthenticator() { }

        public MicrosoftAuthenticator(AuthType authType = AuthType.Access) {
            AuthType = authType;
        }
    }

    partial class MicrosoftAuthenticator {
        public string ClientId { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public AuthType AuthType { get; set; } = AuthType.Access;

        public string[] Scopes => new string[] { "XboxLive.signin", "offline_access", "openid", "profile", "email" };

        protected string AccessToken { get; set; } = string.Empty;
    }
}
