using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;

namespace WonderLab.ViewModels.Pages.Oobe;

public sealed partial class OobeWelcomePageViewModel : ViewModelBase {
    [RelayCommand]
    private void Navigation() {
        WeakReferenceMessenger.Default.Send(new OobePageMessage("OOBELanguage"));
    }
}