using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using MinecraftLaunch.Modules.Toolkits;
using MinecraftLaunch.Modules.Models.Auth;
using System.ComponentModel;
using wonderlab.Views.Pages;
using System;
using wonderlab.control.Controls.Dialog;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace wonderlab.ViewModels.Pages {
    public class DialogPageViewModel : ViewModelBase {
        [Reactive]
        [Obsolete("傻逼玩意没有用")]
        public AccountViewData SelectedAccount { get; set; } = null;

        [Reactive]
        public ObservableCollection<AccountViewData> GameAccounts { set; get; } = null!;

        public DialogPageViewModel() {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            e.PropertyName.ShowLog();
        }

        public void SelectAccountAction() {
            HomePage.ViewModel.CurrentAccount = (AccountDialog.SelectedAccount as AccountViewData)!.Data.ToAccount();
            //此时已选择完账户，直接启动
            HomePage.ViewModel.LaunchTaskAction();
        }
    }
}
