using System;
using System.Threading.Tasks;
using DialogHostAvalonia;
using Microsoft.Extensions.Logging;
using WonderLab.Services.Navigation;
using WonderLab.UI.Controls;
using ZLogger;

namespace WonderLab.Services;

public sealed class DialogService {
    private const string PART_DialogHost = "PART_DialogHost";
    
    private readonly AvaloniaPageProvider _provider;
    private readonly ILogger<DialogService> _logger;
    
    public DialogService(AvaloniaPageProvider provider, ILogger<DialogService> logger) {
        _logger = logger;
        _provider = provider;
    }
    
    public Task<object> ShowDialogByViewAsync<TView>() where TView : DialogContentControl {
        var key = typeof(TView).FullName!;
        var content = _provider.GetPage(key);
        
        _logger.ZLogDebug($"Current key is {key}");
        
        return DialogHost.IsDialogOpen(PART_DialogHost) 
            ? throw new InvalidOperationException("DialogHost can't be opened")
            : DialogHost.Show(content, PART_DialogHost);
    }
    
    public Task<object> ShowDialogByViewModelAsync<TViewModel>() {
        var key = typeof(TViewModel).FullName!;
        var content = _provider.GetPage(key);
        
        _logger.ZLogDebug($"Current key is {key}");
        
        return DialogHost.IsDialogOpen(PART_DialogHost) 
            ? throw new InvalidOperationException("DialogHost can't be opened")
            : DialogHost.Show(content, PART_DialogHost);
    }

    public void Close(object parameter) {
        _logger.ZLogDebug($"{parameter}");
        
        if(DialogHost.IsDialogOpen(PART_DialogHost))
            DialogHost.Close(PART_DialogHost, parameter);
        else 
            throw new InvalidOperationException("DialogHost can't be closed");
    }
}