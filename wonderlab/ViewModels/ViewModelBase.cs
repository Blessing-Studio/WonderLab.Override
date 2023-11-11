using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Utils;
using wonderlab.control.Animation;
using wonderlab.Views.Pages;
using wonderlab.Views.Windows;

namespace wonderlab.ViewModels {
    public class ViewModelBase : ReactiveObject {
        public Dispatcher Dispatcher => Dispatcher.UIThread;

        public virtual void GoBackAction() {
            App.CurrentWindow.OpenTopBar();
            new HomePage().Navigation();

            if (GlobalResources.LauncherData.BakgroundType is not "亚克力背景" or "云母背景（Win11+）") {
                OpacityChangeAnimation animation = new(true);
                animation.RunAnimation(App.CurrentWindow.Back);
            }
        }

        public void AccessAndInvoke(Action action) {
            if (Dispatcher.CheckAccess()) {
                action();
            } else {
                Dispatcher.Post(action);
            }
        }

        public void AccessAndInvoke(Action action, DispatcherPriority priority) {
            if (Dispatcher.CheckAccess()) {
                action();
            } else {
                Dispatcher.Post(action, priority);
            }
        }
    }
}
