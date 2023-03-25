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
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages
{
    public class UserPageViewModel : ReactiveObject {
        public UserPageViewModel() {
            PropertyChanged += OnPropertyChanged;
            Init();
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {       
            
        }

        [Reactive]
        public ObservableCollection<UserModel> GameAccounts { get; set; }

        [Reactive]
        public UserModel CurrentGameAccount { get; set; }

        public void CreateAccountAction() {
            MainWindow.Instance.Auth.Start();
        }

        public async void Init() { 
            GameAccounts = (await GameAccountUtils.GetUsersAsync()).ToObservableCollection();
        }
        public void BackPageAction() {
            MainWindow.Instance.NavigationPage(new ActionCenterPage());
        }
    }
}
