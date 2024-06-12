using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WonderLab.Classes.Datas;
using WonderLab.Services;
using WonderLab.Services.UI;

namespace WonderLab.ViewModels.Windows;

public sealed partial class LogWindowViewModel : ViewModelBase {
    [ObservableProperty] private ReadOnlyObservableCollection<LogData> _logs;

    public LogWindowViewModel(LogService logService, WindowService windowService) {
        Logs = new(logService.LogDatas);
        logService.Info(nameof(LogWindowViewModel), $"日志加载完毕，共加载了 [{logService.LogDatas.Count}] 条");
        logService.Info(nameof(LogWindowViewModel), $"开始加载主窗口");
    }
}