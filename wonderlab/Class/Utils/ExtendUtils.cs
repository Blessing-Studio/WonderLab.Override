using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using wonderlab.Class.Models;
using wonderlab.Class.ViewData;
using Avalonia.Media.Imaging;
using System.Text.RegularExpressions;
using static wonderlab.control.Controls.Bar.MessageTipsBar;
using Avalonia.Controls;
using Image = SixLabors.ImageSharp.Image;
using MinecraftProtocol;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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

        public static bool IsChinese(this string input) => Regex.IsMatch(input, "[\u4e00-\u9fbb]");

        public static double ToDouble(this object obj) { 
            return Convert.ToDouble(obj);
        }

        public static int ToInt32(this object obj) {       
            return Convert.ToInt32(obj);
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
              => (Activator.CreateInstance(typeof(TResult), data!)! as TResult)!;

        public static bool CanUpdate(this UpdateInfo info) {
            if (string.IsNullOrEmpty(info.Title)) {
                return false;
            }

            if(info.Title is not UpdateUtils.VersionType) { 
                return false;
            }

            var intVersion = Convert.ToInt32(info.TagName.Replace(".", string.Empty));
            return intVersion > App.LauncherData.LauncherVersion;
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

        public static JavaInfo? ToJava(this string path) { 
            if (!string.IsNullOrEmpty(path) && path.IsFile()) {
                var info = new FileInfo(path);
                return JavaToolkit.GetJavaInfo(Path.Combine(info.Directory!.FullName, SystemUtils.IsWindows ? "java.exe" : "java"));
            }

            return null;
        }

        public static string ToJavaw(this string path) {
            if (!string.IsNullOrEmpty(path) && path.IsFile()) {           
                var info = new FileInfo(path);
                return Path.Combine(info.Directory!.FullName, SystemUtils.IsWindows ? "javaw.exe" : "javaw");
            }

            return path;
        }

        public static FileInfo ToFile(this string path) {       
            if (!string.IsNullOrEmpty(path) && path.IsFile()) {           
                var info = new FileInfo(path);
                return new(Path.Combine(info.Directory!.FullName, SystemUtils.IsWindows ? "javaw.exe" : "javaw"));
            }

            return new(path);
        }

        public static DirectoryInfo? ToDirectory(this string path) {       
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

        public static Bitmap ToBitmap<TPixel>(this Image<TPixel> raw) where TPixel : unmanaged, IPixel<TPixel> {       
            using var stream = new MemoryStream();
            raw.Save(stream, new PngEncoder());
            stream.Position = 0;
            return new Bitmap(stream);
        }

        public static Bitmap ToBitmap(this Stream stream) {       
            return new Bitmap(stream);
        }

        public static Image<Rgba32> ToImage(this byte[] raw) {       
            return (Image<Rgba32>)Image.Load(raw);
        }

        public static Image<Rgba32> ToImage(this Stream raw) {
            return (Image<Rgba32>)Image.Load(raw);
        }

        public static bool MoveToFront<T>(this List<T> list, T item) {       
            if (list.Count == 0) {           
                return false;
            }

            var index = list.IndexOf(item);
            if (index == -1) {           
                return false;
            }

            var temp = list[index];
            list.RemoveAt(index);
            list.Insert(0, item);

            return true;
        }

        public static string ToModrinthProjectType<T1, T2>(this KeyValuePair<T1, T2> key) {
            return key.Value?.ToString() switch {
                "资源包" => "resourcepack",
                "模组" => "mod",
                "整合包" => "modpack",
                _ => "shader"
            };
        }
        //resourcepack mod modpack shader
        [Obsolete]
        public static Bitmap ToReSizeBitmap(this MemoryStream stream, int width, int hight) {
            using var memoryStream = new MemoryStream();

            Bitmap.DecodeToWidth(stream, width).Save(memoryStream);
            return Bitmap.DecodeToHeight(memoryStream, hight);
        }

        public static async void WriteCompressedText(this string path, string? contents) {       
            if (!File.Exists(path)) {
                path.ToFile().Create();
            }

            if (contents == null) {
                contents = string.Empty;
            }

            byte[] tmp = zlib.Compress(Encoding.UTF8.GetBytes(contents));
            await File.WriteAllBytesAsync(path, tmp);
        }

        public static async ValueTask<string> ReadCompressedText(this string path) {       
            if (!File.Exists(path)) {
                return string.Empty;
            }

            return Encoding.UTF8.GetString(zlib.Decompress(await File.ReadAllBytesAsync(path)));
        }

        public static void ShowLog<T>(this T log) => Trace.WriteLine($"[信息] {log}");

        public static void Navigation(this UserControl control) => MainWindow.Instance.Navigation(control);
    }
}
