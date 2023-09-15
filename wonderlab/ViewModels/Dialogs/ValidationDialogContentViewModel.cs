using Avalonia.Controls;
using DialogHostAvalonia;
using MinecraftLaunch.Modules.Enum;
using MinecraftOAuth.Authenticator;
using MinecraftOAuth.Module.Enum;
using ReactiveUI.Fody.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Utils;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Dialogs {
    public class ValidationDialogContentViewModel : ViewModelBase {
        private string ValidationLink;

        [Reactive]
        public double AnimationWidth { get; set; } = 550;

        [Reactive]
        public double AnimationHeight { get; set; } = 250;

        [Reactive]
        public bool IsSelectNow { get; set; } = true;

        [Reactive]
        public bool IsCodeLoading { get; set; } = false;

        [Reactive]
        public bool IsOfflineShow { get; set; } = false;

        [Reactive]
        public bool IsYggdrasilShow { get; set; } = false;
        
        [Reactive]
        public bool IsCodeLoaded { get; set; } = false;

        [Reactive]
        public string DeviceCode { get; set; }

        [Reactive]
        public string YggdrasilUri { get; set; }

        [Reactive]
        public string Email { get; set; }

        [Reactive]
        public string Password { get; set; }

        [Reactive]
        public string UserName { get; set; }

        public async void GoMicrosoftAuthAction() {
            AnimationWidth = 180;
            AnimationHeight = 60;
            IsCodeLoading = true;
            IsSelectNow = false;

            MicrosoftAuthenticator authenticator = new(AuthType.Access) {
                ClientId = GlobalResources.ClientId
            };

            var code = await authenticator.GetDeviceInfo();

            if (code is not null) {
                DeviceCode = code.UserCode;
                ValidationLink = code.VerificationUrl;

                AnimationWidth = 560;
                AnimationHeight = 120;
                IsCodeLoading = false;
                IsCodeLoaded = true;

                var token = await authenticator.GetTokenResponse(code);
                var account = await authenticator.AuthAsync(x => {
                    x.ShowLog();
                });

                await AccountUtils.SaveAsync(new() {
                    AccessToken = account.RefreshToken!,
                    UserName = account.Name,
                    UserToken = account.AccessToken,
                    Uuid = account.Uuid.ToString(),
                    UserType = account.Type
                }, true);

                $"账户 {account.Name} 已成功添加至启动器".ShowMessage();
                await Task.Run(() => {
                    AccountPage.ViewModel.GameAccounts = CacheResources.Accounts;
                });

                CloseAction();
            }
        }

        public override void GoBackAction() {
            IsSelectNow = true;
            AnimationWidth = 550;
            AnimationHeight = 250;

            IsOfflineShow = false;
            IsCodeLoading = false;
            IsCodeLoaded = false;
            IsYggdrasilShow = false;
        }

        public async void GoValidationLinkAction() {
            await TopLevel.GetTopLevel(App.CurrentWindow).Clipboard!.SetTextAsync(DeviceCode);
            using var process = Process.Start(new ProcessStartInfo(ValidationLink) {
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public void GoOfflineAuthAction() {
            AnimationWidth = 350;
            AnimationHeight = 70;
            IsSelectNow = false;
            IsOfflineShow = true;
        }

        public void GoYggdrasilAuthAction() {
            AnimationWidth = 350;
            AnimationHeight = 160;

            IsSelectNow = false;
            IsYggdrasilShow = true;
        }

        public void CloseAction() {
            DialogHost.Close("dialogHost");
        }

        public async void YggdrasilAuthAction() {
            if (!RegexUtils.EmailCheck.IsMatch(Email)) {
                "不正确的邮箱地址，请校正后再次尝试！".ShowMessage("提示");
                return;
            }

            YggdrasilAuthenticator authenticator = new(YggdrasilUri, Email, Password);
            try {
                var result = await authenticator.AuthAsync();
                foreach (var account in result.AsParallel()) {
                    await AccountUtils.SaveAsync(new() {
                        Email = account.Email,
                        UserName = account.Name,
                        UserType = account.Type,
                        Password = account.Password,
                        Uuid = account.Uuid.ToString(),
                        UserToken = account.AccessToken,
                        AccessToken = account.ClientToken!,
                        YggdrasilUrl = account.YggdrasilServerUrl
                    }, true);
                }
            }
            catch (Exception ex) {
                $"登录失败，请检查您的信息是否填写正确，详细信息如下：{ex}".ShowInfoDialog("登录失败");
            }

            $"已成功将 {Email} 名下所有的账户全部添加至启动器".ShowMessage();
            CloseAction();
        }

        public async void OfflineAuthAction() {
            OfflineAuthenticator authenticator = new(UserName);
            var account = authenticator.Auth();

            await AccountUtils.SaveAsync(new() {
                UserToken = account.AccessToken,
                UserName = account.Name,
                Uuid = account.Uuid.ToString(),
            }, true);

            $"账户 {account.Name} 已成功添加至启动器".ShowMessage();
            CloseAction();
        }
    }
}
