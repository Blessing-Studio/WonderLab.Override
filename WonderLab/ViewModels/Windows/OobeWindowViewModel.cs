using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Utilities;
using System.Threading.Tasks;

namespace WonderLab.ViewModels.Windows;

public sealed partial class OobeWindowViewModel : ViewModelBase {
    [ObservableProperty] private bool _isTitleBarVisible;
    [ObservableProperty] private bool _isOpenBackgroundPanel;

    public OobeWindowViewModel() {
        IsTitleBarVisible = EnvironmentUtil.IsWindow;
        Task.Run(async () => {
            await Task.Delay(800);
            IsOpenBackgroundPanel = true;
        });
    }
}