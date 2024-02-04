using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WonderLab.Classes.Utilities;

public class DialogUtil
{
    private readonly static IStorageProvider StorageProvider =
        App.StorageProvider;

    public static async ValueTask<FileInfo> OpenFilePickerAsync(IEnumerable<FilePickerFileType> filters, string title)
    {
        var result = await StorageProvider.OpenFilePickerAsync(new()
        {
            AllowMultiple = false,
            FileTypeFilter = filters.ToList(),
            Title = title
        });

        if (result is null || !result.Any())
        {
            return null!;
        }

        return new(result.First().Path.LocalPath);
    }

    public static async ValueTask<DirectoryInfo> OpenFolderPickerAsync(string title)
    {
        var result = await StorageProvider.OpenFolderPickerAsync(new()
        {
            AllowMultiple = false,
            Title = title
        });

        if (result is null || !result.Any())
        {
            return null!;
        }

        return new(result.First().Path.LocalPath);
    }

    public static async ValueTask<FileInfo> SaveFilePickerAsync(string title, string fileName)
    {
        var result = await StorageProvider.SaveFilePickerAsync(new()
        {
            Title = title,
            SuggestedFileName = fileName,
        });

        if (result is null)
        {
            return null!;
        }

        return new(result.Path.LocalPath);
    }
}
