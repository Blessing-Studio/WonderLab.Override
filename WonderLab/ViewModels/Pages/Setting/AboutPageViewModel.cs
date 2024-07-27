using CommunityToolkit.Mvvm.Input;
using Flurl;
using System.Diagnostics;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class AboutPageViewModel : ViewModelBase {
    [RelayCommand]
    private void JumpToLink(string url) {
        RunBackgroundWork(() => {
            using var _ = Process.Start(new ProcessStartInfo(url) {
                UseShellExecute = true,
                Verb = "open"
            });
        });
    }
}