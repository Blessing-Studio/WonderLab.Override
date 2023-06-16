using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.Views.Pages;
using MinecraftLaunch.Modules.Toolkits;
using System.Linq;
using wonderlab.control;
using System.Threading.Tasks;
using System.ComponentModel;
using wonderlab.Class.AppData;

namespace wonderlab.ViewModels.Pages {
    public class AccountPageViewModel : ViewModelBase {
        public AccountPageViewModel() {
            Init();
        }

        [Reactive]
        public ObservableCollection<AccountViewData> GameAccounts { get; set; } = new();

        [Reactive]
        public AccountViewData CurrentGameAccount { get; set; } = null;

        [Reactive]
        public bool IsSelectAccount { get; set; }

        public void CreateAccountAction() {
            App.CurrentWindow.DialogHost.AccountTypeDialog.ShowDialog();
        }

        public async void Init() {
            PropertyChanged += OnPropertyChanged;            
            await Task.Run(async () => {
                GameAccounts = (await AccountUtils.GetAsync(true).ToListAsync()).ToObservableCollection();
                CacheResources.Accounts = GameAccounts;
            });
        }

        public void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(CurrentGameAccount)) {
                IsSelectAccount = !CurrentGameAccount.IsNull();
            }
        }

        public override void GoBackAction() {
            new ActionCenterPage().Navigation();
        }
    }
}
