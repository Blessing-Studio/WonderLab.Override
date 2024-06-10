using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;
using WonderLab.Views.Pages.Oobe;

namespace WonderLab.ViewModels.Pages.Oobe;

public sealed partial class OobeWelcomePageViewModel : ViewModelBase {
    [RelayCommand]
    private void Navigation() {
        WeakReferenceMessenger.Default.Send(new OobePageMessage {
            PageKey = "OOBELanguage"
        });
    }
}