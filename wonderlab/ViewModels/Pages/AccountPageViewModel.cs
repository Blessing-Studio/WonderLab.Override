using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.Views.Pages;
using MinecraftLaunch.Modules.Toolkits;

namespace wonderlab.ViewModels.Pages {
    public class AccountPageViewModel : ViewModelBase {
        public AccountPageViewModel() {
            Init();
        }

        [Reactive]
        public ObservableCollection<AccountViewData> GameAccounts { get; set; } = new();

        [Reactive]
        public AccountViewData CurrentGameAccount { get; set; }

        public void CreateAccountAction() {
            App.CurrentWindow.Auth.Start();
        }

        public void Init() {
            Dispatcher.UIThread.Post(async () => {
                GameAccounts = (await AccountUtils.GetAsync().ToListAsync()).ToObservableCollection();
            });
        }
        public override void GoBackAction() {
            new ActionCenterPage().Navigation();
        }
    }
}
