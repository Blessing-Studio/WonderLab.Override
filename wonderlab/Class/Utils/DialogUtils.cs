using Avalonia.Platform.Storage;
using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.AppData;

namespace wonderlab.Class.Utils {
    public class DialogUtils {
        private static Uri PathCache = null!;

        private readonly static IStorageProvider StorageProvider = App.CurrentWindow.StorageProvider;

        public static async ValueTask<FileInfo> OpenFilePickerAsync(IEnumerable<FilePickerFileType> filters, string title) {
            var result = await StorageProvider.OpenFilePickerAsync(new() {
                AllowMultiple = false,
                FileTypeFilter = filters.ToList(),
                Title = title
            });

            if (result.IsNull() || !result.Any()) {
                return null!;
            }

            result.First().TryGetUri(out PathCache!);
            if (!PathCache.IsNull() && PathCache.LocalPath.IsFile()) {
                return PathCache.LocalPath.ToFile();
            }

            return null!;
        }

        public static async ValueTask<DirectoryInfo> OpenFolderPickerAsync(string title) {
            var result = await StorageProvider.OpenFolderPickerAsync(new() {
                AllowMultiple = false,
                Title = title                
            });

            if (result.IsNull() || !result.Any()) {
                return null!;
            }

            result.First().TryGetUri(out PathCache!);
            if (!PathCache.IsNull() && PathCache.LocalPath.IsDirectory()) {
                return PathCache.LocalPath.ToDirectory()!;
            }

            return null!;
        }

        public static async ValueTask<FileInfo> SaveFilePickerAsync(string title,string fileName) {
            var result = await StorageProvider.SaveFilePickerAsync(new() {
                Title = title,
                SuggestedFileName = fileName,
            });

            if (result!.IsNull()) {
                return null!;
            }

            result!.TryGetUri(out PathCache!);
            if (!PathCache.IsNull() && PathCache.LocalPath.IsFile()) {
                return PathCache.LocalPath.ToFile();
            }

            return null!;
        }
    }
}
