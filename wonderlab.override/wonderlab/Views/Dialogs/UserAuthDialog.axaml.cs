using Avalonia.Controls;
using Avalonia.Input;
using MinecraftLaunch.Modules.Models.Auth;
using System;
using System.Threading;
using wonderlab.Class.Utils;
using wonderlab.ViewModels.Dialogs;

namespace wonderlab.Views.Dialogs
{
    public partial class UserAuthDialog : UserControl
    {
        public static UserAuthDialogViewModel ViewModel { get; set; } = new();

        public UserAuthDialog()
        {
            InitializeComponent();
            DataContext = ViewModel;
            
            offline.Click += (_, _) => {
                TypeSelecter.HideDialog();
                ViewModel.CurrentAuthenticatorType = 0;
                AuthDialog.ShowDialog();
            };

            yggdrasil.Click += (_, _) => {
                TypeSelecter.HideDialog();
                ViewModel.CurrentAuthenticatorType = 1;
                AuthDialog.ShowDialog();
            };

            microsoft.Click += (_, _) => {
                TypeSelecter.HideDialog();
                ViewModel.CurrentAuthenticatorType = 2;
                AuthDialog.ShowDialog();
            };

            close.Click += (_, _) => {
                Close();
            };

            closeTypeSelecter.Click += (_, _) => {
                TypeSelecter.HideDialog();
            };

            YggUrl.AddHandler(DragDrop.DropEvent, (_, x) => { 
                if(x.Data.Contains(DataFormats.Text)) {
                    ViewModel.Url = x.Data.GetText()!.Replace("authlib-injector:yggdrasil-server:", string.Empty).Replace("%2F", "/").Replace("%3A", ":");
                }
            });
        }

        public void Start() {
            TypeSelecter.ShowDialog();
        }

        public void Close() {
            AuthDialog.HideDialog();
        }

        public async void Show() {
            ViewModel.GameAccounts = (await GameAccountUtils.GetUsersAsync().ToListAsync()).ToObservableCollection();
            AccountSelector.ShowDialog();
        }
    }
}
