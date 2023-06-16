using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages {
    public partial class AccountPage : UserControl {
        public static AccountPageViewModel ViewModel { get; private set; } = new();

        public AccountPage() {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private async void OnAccountDeleteClick(object? sender, RoutedEventArgs e) {
            try {
                var userInfo = ((AccountViewData)(sender as MenuItem)!.DataContext!).Data;
                await AccountUtils.DeleteAsync(userInfo);
                ViewModel.GameAccounts = CacheResources.Accounts;
            }
            catch (Exception ex) {
                $"{ex}".ShowInfoDialog("≥Ã–Ú‘‚”ˆ¡À“Ï≥£");
            }
        }
    }
}
