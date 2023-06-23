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

namespace wonderlab.ViewModels.Pages {
    public class DialogPageViewModel : ViewModelBase {
        private string ValidationLink { set; get; }
        private ValidationDialog.ValidationTypes CurrentType { set; get; }

        public UpdateInfo info = null;

        [Reactive]
        [Obsolete("傻逼玩意没有用")]
        public AccountViewData SelectedAccount { get; set; } = null;

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
            await Application.Current!.Clipboard!.SetTextAsync(DeviceCodeText);
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
                        YggdrasilAuthenticator authenticator = new(uri, email, password);
                        var result = await authenticator.AuthAsync();
                        $"已成功将 {email} 名下所有的账户全部添加至启动器".ShowMessage();

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
                });

                await Task.Run(async () => {
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
            App.CurrentWindow.DialogHost.Validation.HideDialog();
        }
    }
}
