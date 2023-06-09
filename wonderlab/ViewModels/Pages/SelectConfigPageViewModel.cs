﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Utils;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages {
    public class SelectConfigPageViewModel : ViewModelBase {
        public SelectConfigPageViewModel() {
            PropertyChanged += OnPropertyChanged;
        }

        [Reactive]
        public object SelectConfigPage { get; set; } = new LaunchConfigPage();

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        }

        public void GoLaunchConfigPageAction() {
            SelectConfigPage = new LaunchConfigPage();
        }

        public void GoPersonalizeConfigPageAction() {
            SelectConfigPage = new PersonalizeConfigPage();
        }

        public void GoWebConfigPageAction() {
            SelectConfigPage = new NetConfigPage();
        }

        public void GoAboutPageAction() {
            new AboutPage().Navigation();
        }

        public override void GoBackAction() {
            new ActionCenterPage().Navigation();
        }
    }
}
