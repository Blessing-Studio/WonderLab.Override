using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.control.Animation;
using wonderlab.control.Controls.Bar;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages
{
    public class ActionCenterPageViewModel : ReactiveObject
    {
        public ActionCenterPageViewModel() {
            PropertyChanged += OnPropertyChanged;
        }

        public void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {       
            throw new NotImplementedException();
        }

        public void ReturnAction() {
            MainWindow.Instance.NavigationPage(new HomePage());
            var transform = MainWindow.Instance.OpenBar!.RenderTransform as TranslateTransform;

            OpacityChangeAnimation animation = new(true);
            TranslateXAnimation animation2 = new(transform.X, 0);
            animation2.RunAnimation(MainWindow.Instance.OpenBar);

            TranslateXAnimation animation1 = new(100, 0);
            animation1.RunAnimation(MainWindow.Instance.ToolBar);
            animation.RunAnimation(MainWindow.Instance.Back);
        }
    }
}
