using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Utils;
using wonderlab.control.Animation;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages {
    public class SelectConfigPageViewModel : ViewModelBase {
        private bool Isswitch = false;

        private ContentControl LeftContent;

        private ContentControl RightContent;

        private int oldIndex = 0;

        private readonly PageVaryAnimation varyAnimation = new(TimeSpan.FromMilliseconds(500));

        public SelectConfigPageViewModel(ContentControl left, ContentControl right) {
            PropertyChanged += OnPropertyChanged;

            LeftContent = left;
            RightContent = right;
        }

        [Reactive]
        public object LeftSelectConfigPage { get; set; } = new LaunchConfigPage();

        [Reactive]
        public object RightSelectConfigPage { get; set; }

        public int CurrentPageIndex { get; set; } = 0;

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        }

        public void GoLaunchConfigPageAction() {
            CurrentPageIndex = 0;
            RunVaryAnimation(new LaunchConfigPage());
        }

        public void GoWebConfigPageAction() {
            CurrentPageIndex = 1;
            RunVaryAnimation(new NetConfigPage());
        }

        public void GoPersonalizeConfigPageAction() {
            CurrentPageIndex = 2;
            RunVaryAnimation(new PersonalizeConfigPage());
        }

        public void GoAboutPageAction() {
            new AboutPage().Navigation();
        }

        public override void GoBackAction() {
            new ActionCenterPage().Navigation();
        }

        public void RunVaryAnimation(object to) {
            if (oldIndex == CurrentPageIndex) {
                return;
            }

            if (!Isswitch) {
                RightSelectConfigPage = to;
                _ = varyAnimation.Start(LeftContent, RightContent,
                    oldIndex < CurrentPageIndex, default);
            } else {
                LeftSelectConfigPage = to;
                _ = varyAnimation.Start(RightContent, LeftContent, oldIndex < CurrentPageIndex, default);
            }

            Isswitch = !Isswitch;
            oldIndex = CurrentPageIndex;
        }
    }
}
