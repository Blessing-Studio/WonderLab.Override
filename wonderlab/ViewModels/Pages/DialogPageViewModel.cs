using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using System.ComponentModel;
using wonderlab.Views.Pages;
using System;
using wonderlab.control.Controls.Dialog;
using wonderlab.Class.AppData;
using System.Diagnostics;
using Avalonia;
using System.Linq;
using System.Threading.Tasks;
using MinecraftLaunch.Modules.Authenticator;
using MinecaftOAuth.Module.Enum;
using Avalonia.Controls;

namespace wonderlab.ViewModels.Pages {
    public class DialogPageViewModel : ViewModelBase {
        private string ValidationLink { set; get; }
        private ValidationDialog.ValidationTypes CurrentType { set; get; }

        public VersionInfo info = null;

        [Reactive]
        [Obsolete("傻逼玩意没有用")]
        public AccountViewData SelectedAccount { get; set; } = null!;

        [Reactive]
        public ObservableCollection<AccountViewData> GameAccounts { set; get; } = null!;

        [Reactive]
        public bool HasGame { get; set; } = false;

        [Reactive]
        public bool IsCodeLoading { get; set; }

        [Reactive]
        public string DeviceCodeText { get; set; }

        [Reactive]
        public double DownloadProgress { get; set; } = 0.0;

        [Reactive]
        public bool IsForgeLoaded { get; set; } = false;

        [Reactive]
        public bool IsFabricLoaded { get; set; } = false;

        [Reactive]
        public bool IsOptifineLoaded { get; set; } = false;

        [Reactive]
        public bool IsQuiltLoaded { get; set; } = false;

        [Reactive]
        public bool SelectedLoader { get; set; } = false;

        public DialogPageViewModel() {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            e.PropertyName.ShowLog();
        }

        public void SelectAccountAction() {
            HomePage.ViewModel.CurrentAccount = (AccountDialog.SelectedAccount as AccountViewData)!.Data.ToAccount();
            //此时已选择完账户，直接启动
            HomePage.ViewModel.LaunchTaskAction();
        }

        public void OfflineSelectedAction() {
            CurrentType = App.CurrentWindow.DialogHost.Validation.ShowDialog(ValidationDialog.ValidationTypes.Offline);
        }

        public void YggdrasilSelectedAction() {
            CurrentType = App.CurrentWindow.DialogHost.Validation.ShowDialog(ValidationDialog.ValidationTypes.Yggdrasil);
        }

        public async void MicrosoftSelectedAction() {
            App.CurrentWindow.DialogHost.Validation.ShowDialog(ValidationDialog.ValidationTypes.Microsoft);
            MicrosoftAuthenticator authenticator = new() {
                ClientId = GlobalResources.ClientId,
                AuthType = AuthType.Access
            };

            try {
                var deviceCodeInfo = await authenticator.GetDeviceInfo();
                if (!deviceCodeInfo.IsNull()) {
                    DeviceCodeText = deviceCodeInfo.UserCode;
                    App.CurrentWindow.DialogHost.Validation.IsCodeLoading = false;
                    ValidationLink = deviceCodeInfo.VerificationUrl;
                }

                var token = await authenticator.GetTokenResponse(deviceCodeInfo);
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

                CancelAction();
            }
            catch (Exception ex) when (ex is TimeoutException) {
                App.CurrentWindow.DialogHost.Validation.HideDialog();
                "无法完成此次验证操作，原因：验证超时！".ShowInfoDialog("错误");
                CancelAction();
                return;
            }
            catch (Exception ex) when (ex is NotSupportedException) {
                HasGame = true;
                return;
            }
            catch (Exception ex) {
                $"无法完成此次验证操作，原因：{ex}".ShowInfoDialog("错误");
            }        
        }

        public async void GoValidationLinkAction() {
            await TopLevel.GetTopLevel(App.CurrentWindow).Clipboard.SetTextAsync(DeviceCodeText);
            using var process = Process.Start(new ProcessStartInfo(ValidationLink) {
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public async void GoWriteAction() {
            var validation = App.CurrentWindow.DialogHost.Validation;
            string email = validation.Email, uri = validation.YggdrasilUri, password = validation.Password;

            try {
                await Task.Run(async () => {
                    if (CurrentType is ValidationDialog.ValidationTypes.Offline) {
                        OfflineAuthenticator authenticator = new(email);
                        var account = authenticator.Auth();

                        await AccountUtils.SaveAsync(new() {
                            UserToken = account.AccessToken,
                            UserName = account.Name,
                            Uuid = account.Uuid.ToString(),
                        }, true);

                        $"账户 {account.Name} 已成功添加至启动器".ShowMessage();
                    } else {
                        if (!RegexUtils.EmailCheck.IsMatch(email)) {
                            "不正确的邮箱地址，请校正后再次尝试！".ShowMessage("提示");
                            return;
                        }

                        YggdrasilAuthenticator authenticator = new(uri, email, password);
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

                        $"已成功将 {email} 名下所有的账户全部添加至启动器".ShowMessage();
                    }
                });

                await Task.Run(() => {
                    AccountPage.ViewModel.GameAccounts = CacheResources.Accounts;
                });
            }
            catch (Exception ex) {
                $"WonderLab 在登录账户时遭遇了异常，详细信息：{ex}".ShowInfoDialog("程序遭遇了异常");
            }

            CancelAction();
        }

        public void UpdateAction() {
            UpdateUtils.UpdateAsync(info, x => {
                DownloadProgress = x.ToDouble() * 100;
            });
        }

        public void CancelAction() {
            HasGame = false;            
            App.CurrentWindow.DialogHost.Validation.HideDialog();
        }
    }
}
