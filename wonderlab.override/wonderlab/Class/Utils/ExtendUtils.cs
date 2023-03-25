using Avalonia.Data.Core;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.ViewData;
using static wonderlab.control.Controls.Bar.MessageTipsBar;

namespace wonderlab.Class.Utils
{
    public static class ExtendUtils
    {
        public static void ShowMessage(this string message) {
            MainWindow.Instance?.ShowInfoBar("信息", message);
        }

        public static void ShowMessage(this string message,string title) {       
            MainWindow.Instance?.ShowInfoBar(title, message);
        }

        public static void ShowMessage(this string message, HideOfRunAction action) {
            MainWindow.Instance?.ShowInfoBar("信息", message, action);
        }

        public static void ShowMessage(this string message, string title,HideOfRunAction action) {       
            MainWindow.Instance?.ShowInfoBar(title, message, action);
        }

        public static double ToDouble(this object obj) { 
            return Convert.ToDouble(obj);
        }

        public static IEnumerable<T> ToEnumerable<T>(this IAsyncEnumerable<T> obj) {
            var enumerator = obj.GetAsyncEnumerator();

            while (enumerator.MoveNextAsync().Result) {           
                yield return enumerator.Current;
            }
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> obj) {             
            return new(obj.Select(x => x).Distinct());
        }

        public static TResult CreateViewData<TData, TResult>(this TData data) where TResult : ViewDataBase<TData>
              => Activator.CreateInstance(typeof(TResult), data)! as TResult;

        public static bool CanUpdate(this UpdateInfo info) {
            if (string.IsNullOrEmpty(info.Title)) {
                return false;
            }

            if(info.Title is not UpdateUtils.VersionType) { 
                return false;
            }

            var intVersion = Convert.ToInt32(info.TagName.Replace(".", string.Empty));
            return intVersion > UpdateUtils.Version;
        }

        public static ModLoaderViewData GetForge(this ObservableCollection<ModLoaderViewData> data) {
            if (data.First().Data.ModLoaderType == ModLoaderType.Forge) { 
                return data.First();
            }

            return data.Last();
        }

        public static ModLoaderViewData GetOptiFine(this ObservableCollection<ModLoaderViewData> data) {       
            if (data.First().Data.ModLoaderType == ModLoaderType.OptiFine) {           
                return data.First();
            }

            return data.Last();
        }

        public static JavaInfo ToJava(this string path) { 
            if (!string.IsNullOrEmpty(path) && path.IsFile()) {
                var info = new FileInfo(path);
                return JavaToolkit.GetJavaInfo(Path.Combine(info.Directory.FullName, SystemUtils.IsWindows ? "java.exe" : "java"));
            }

            return null;
        }

        public static string ToJavaw(this string path) {
            if (!string.IsNullOrEmpty(path) && path.IsFile()) {           
                var info = new FileInfo(path);
                return Path.Combine(info.Directory.FullName, SystemUtils.IsWindows ? "javaw.exe" : "javaw");
            }

            return path;
        }

        public static FileInfo ToFile(this string path) {       
            if (!string.IsNullOrEmpty(path) && path.IsFile()) {           
                var info = new FileInfo(path);
                return new(Path.Combine(info.Directory.FullName, SystemUtils.IsWindows ? "javaw.exe" : "javaw"));
            }

            return new(path);
        }

        public static DirectoryInfo ToDirectory(this string path) {       
            if (!string.IsNullOrEmpty(path) && path.IsDirectory()) {           
                return new(path);
            }

            return null;
        }

        public static Account ToAccount(this UserModel user) { 
            return user.UserType switch { 
                AccountType.Offline => new OfflineAccount() {   
                    Name = user.UserName,
                    Uuid = Guid.Parse(user.Uuid),
                    AccessToken = user.UserToken,
                    Type = AccountType.Offline,                    
                },

                AccountType.Microsoft => new MicrosoftAccount() {
                    Name = user.UserName,
                    Uuid = Guid.Parse(user.Uuid),
                    AccessToken = user.UserToken,
                    Type = AccountType.Microsoft,
                    RefreshToken = user.AccessToken,                   
                },

                AccountType.Yggdrasil => new YggdrasilAccount() {
                    Name = user.UserName,
                    Uuid = Guid.Parse(user.Uuid),
                    AccessToken = user.UserToken,
                    ClientToken = user.AccessToken,
                    Email = user.Email,
                    Password = user.Password,
                    YggdrasilServerUrl = user.YggdrasilUrl,
                    Type = AccountType.Microsoft,
                },

                _ => Account.Default
            };
        }
    }
}
