﻿using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using DialogHostAvalonia;
using WonderLab.ViewModels;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using WonderLab.Views.Dialogs.Setting;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.Views.Dialogs;

namespace WonderLab.Services.UI;

public sealed class DialogService {
    private readonly WindowService _windowService;
    private readonly Dictionary<string, Func<object>> _dialogs = new() {
        { nameof(TestUserCheckDialog), App.ServiceProvider.GetRequiredService<TestUserCheckDialog> },
        { nameof(ChooseAccountTypeDialog), App.ServiceProvider.GetRequiredService<ChooseAccountTypeDialog> },
        { nameof(OfflineAuthenticateDialog), App.ServiceProvider.GetRequiredService<OfflineAuthenticateDialog> },
        { nameof(YggdrasilAuthenticateDialog), App.ServiceProvider.GetRequiredService<YggdrasilAuthenticateDialog> },
    };

    public bool IsDialogOpen => DialogHost.IsDialogOpen("dialogHost");

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

    public async void ShowContentDialog<TViewModel>() where TViewModel : ViewModelBase {
        if (DialogHost.IsDialogOpen("dialogHost")) {
            return;
        }

        var viewName = typeof(TViewModel).Name.Replace("ViewModel", "");

        if (_dialogs.TryGetValue(viewName, out var contentFunc)) {
            var pageObject = contentFunc() as UserControl;
            pageObject!.DataContext = App.ServiceProvider!.GetRequiredService<TViewModel>();
            await DialogHost.Show(pageObject, "dialogHost");
        }
    }

    public void CloseContentDialog() {
        DialogHost.Close("dialogHost");
    }
}