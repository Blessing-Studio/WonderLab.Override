using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.AppData;

namespace wonderlab.Class.Utils {
    public class DialogUtils {
        private readonly static IStorageProvider StorageProvider = App.CurrentWindow.StorageProvider;

        public static async ValueTask<IStorageFile> OpenFilePickerAsync(IEnumerable<FilePickerFileType> filters, string title) {
            var result = await StorageProvider.OpenFilePickerAsync(new() {
                AllowMultiple = false,
                FileTypeFilter = filters.ToList(),
                Title = title
            });

            if (!result.Any()) {
                return null!;
            }

            return result.First();
        }

        public static async ValueTask<IStorageFolder> OpenFolderPickerAsync(string title) {
            var result = await StorageProvider.OpenFolderPickerAsync(new() {
                AllowMultiple = false,
                Title = title                
            });

            if (!result.Any()) {
                return null!;
            }

            return result.First();
        }

        public static async ValueTask<IStorageFile> SaveFilePickerAsync(IEnumerable<FilePickerFileType> filters, string title,string fileName) {
            var result = await StorageProvider.SaveFilePickerAsync(new() {
                Title = title,
                SuggestedFileName = fileName,
                FileTypeChoices = filters.ToList()
            });

            return result!;
        }

        public static string GetFilePath(IStorageFile file) {
            Uri uri = null!;
            file.TryGetUri(out uri!);

            return uri.LocalPath;
        }

        public static string GetFolderPath(IStorageFolder file) {
            Uri uri = null!;
            file.TryGetUri(out uri!);

            return uri.LocalPath;
        }
    }
}
