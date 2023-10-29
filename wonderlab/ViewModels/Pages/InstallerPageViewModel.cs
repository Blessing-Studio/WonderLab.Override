using Avalonia.Controls;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using wonderlab.Class.Models;
using wonderlab.control.Animation;

namespace wonderlab.ViewModels.Pages {
    public class InstallerPageViewModel : ViewModelBase {
        private Control leftContent; 

        private Control rightContent;

        private bool Isswitch = false;

        private CancellationTokenSource animationToken = new();

        private readonly PageVaryAnimation varyAnimation = new(TimeSpan.FromMilliseconds(500)) {
            IsHorizontal = true,
        };

        public InstallerPageViewModel(Control left, Control right) {      
            leftContent = left;
            rightContent = right;
        }

        [Reactive]
        public ObservableCollection<ModLoaderModel> CurrentLoaders { get; set; } = new();

        public async void GotoAction() {
            leftContent.Opacity = 0;
            leftContent.IsHitTestVisible = false;

            animationToken.Cancel();
            animationToken.Dispose();
            animationToken = new();

            rightContent.Opacity = 1;
            rightContent.IsHitTestVisible = true;

            await varyAnimation.Start(leftContent, rightContent, true, 
                animationToken.Token);
        }

        public async void GobackAction() {
            rightContent.Opacity = 0;
            rightContent.IsHitTestVisible = false;

            animationToken.Cancel();
            animationToken.Dispose();
            animationToken = new();

            leftContent.Opacity = 1;
            leftContent.IsHitTestVisible = true;

            await varyAnimation.Start(rightContent, leftContent, false,
                animationToken.Token);
        }
    }
}
