using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.Views.Pages;
using MinecraftLaunch.Modules.Toolkits;

namespace wonderlab.ViewModels.Pages
{
    public class UserPageViewModel : ReactiveObject {
        public UserPageViewModel() {
            Init();
        }

        [Reactive]
        public ObservableCollection<AccountViewData> GameAccounts { get; set; }

        [Reactive]
        public AccountViewData CurrentGameAccount { get; set; }

        public void CreateAccountAction() {
            MainWindow.Instance.Auth.Start();
        }

        public void Init() {
            Dispatcher.UIThread.Post(async () => {
                GameAccounts = (await GameAccountUtils.GetUsersAsync().ToListAsync()).ToObservableCollection();
            });
        }
        public void BackPageAction() {
            new ActionCenterPage().Navigation();
        }
    }
}
