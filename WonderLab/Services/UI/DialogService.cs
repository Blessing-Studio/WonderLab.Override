using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using System.Collections.Generic;

namespace WonderLab.Services.UI;

public sealed class DialogService {
    private readonly WindowService _windowService;

    public DialogService(WindowService windowService) {
        _windowService = windowService;
    }

    public async ValueTask<FileInfo> OpenFilePickerAsync(IEnumerable<FilePickerFileType> filters, string title) {
        var result = await _windowService.GetStorageProvider().OpenFilePickerAsync(new() {
            AllowMultiple = false,
            FileTypeFilter = filters.ToList(),
            Title = title
        });

        if (result is null || !result.Any()) {
            return null!;
        }

        return new(result.First().Path.LocalPath);
    }

    public async ValueTask<DirectoryInfo> OpenFolderPickerAsync(string title) {
        var result = await _windowService.GetStorageProvider().OpenFolderPickerAsync(new() {
            AllowMultiple = false,
            Title = title
        });

        if (result is null || !result.Any()) {
            return null;
        }

        return new(result[0].Path.LocalPath);
    }

    public async ValueTask<FileInfo> SaveFilePickerAsync(string title, string fileName) {
        var result = await _windowService.GetStorageProvider().SaveFilePickerAsync(new() {
            Title = title,
            SuggestedFileName = fileName,
        });

        if (result is null) {
            return null;
        }

        return new(result.Path.LocalPath);
    }
}