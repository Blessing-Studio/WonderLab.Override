using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;

namespace WonderLab.ViewModels.Pages.Oobe;
public sealed partial class OobeAccountPageViewModel : ViewModelBase {
    [RelayCommand]
    private void Navigation(){
        WeakReferenceMessenger.Default.Send(new OobePageMessage("OOBELanguage"));
    }
}