using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.ViewModels.Pages
{
    public class UserPageViewModel : ReactiveObject {
        public UserPageViewModel() {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {       
            
        }

        public void CreateAccountAction() {
            MainWindow.Instance.Auth.Start();
        }
    }
}
