using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using wonderlab.Class.Models;
using wonderlab.Class.ViewData;
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
        public bool IsForgeLoaded { get; set; }

        [Reactive]
        public bool IsNeoForgeLoaded { get; set; }

        [Reactive]
        public bool IsFabricLoaded { get; set; }

        [Reactive]
        public bool IsOptifineLoaded { get; set; }

        [Reactive]
        public bool IsQuiltLoaded { get; set; }

        [Reactive]
        public ObservableCollection<ModLoaderModel> CurrentLoaders { get; set; } = new();

        public async void GotoAction() {
            await Dispatcher.InvokeAsync(async () => {
                leftContent.Opacity = 0;
                leftContent.IsHitTestVisible = false;

                animationToken.Cancel();
                animationToken.Dispose();
                animationToken = new();

                rightContent.Opacity = 1;
                rightContent.IsHitTestVisible = true;

                await varyAnimation.Start(leftContent, rightContent, true,
                    animationToken.Token);
            }, DispatcherPriority.Send);
        }

        public async void GobackAction() {
            await Dispatcher.InvokeAsync(async () => {
                rightContent.Opacity = 0;
                rightContent.IsHitTestVisible = false;

                animationToken.Cancel();
                animationToken.Dispose();
                animationToken = new();

                leftContent.Opacity = 1;
                leftContent.IsHitTestVisible = true;

                await varyAnimation.Start(rightContent, leftContent, false,
                    animationToken.Token);
            }, DispatcherPriority.Send);
        }
    }
}
