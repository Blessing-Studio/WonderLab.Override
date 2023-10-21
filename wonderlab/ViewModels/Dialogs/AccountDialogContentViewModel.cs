using DialogHostAvalonia;
using MinecraftLaunch.Modules.Models.Auth;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.control.Controls.Dialog;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Dialogs {
    public class AccountDialogContentViewModel : ViewModelBase {
        public AccountDialogContentViewModel() {
            Accounts = CacheResources.Accounts;
        }

        [Reactive]
        public AccountViewData Current { get; set; }

        [Reactive]
        public ObservableCollection<AccountViewData> Accounts { get; set; } = new();

        public override void GoBackAction() {
            DialogHost.Close("dialogHost");
        }

        public void SelectAccountAction() {
            HomePage.ViewModel.CurrentAccount = Current.Data.ToAccount();
            GoBackAction();
            //此时已选择完账户，直接启动
            HomePage.ViewModel.LaunchTaskAction();
        }
    }
}
