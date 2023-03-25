using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.control.Animation;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages
{
    public class GameCoreConfigPageViewModel : ReactiveObject {   
        public GameCoreConfigPageViewModel() {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {       

        }

        public void BackHomePageAction() {
            MainWindow.Instance.NavigationPage(new HomePage());
            var transform = MainWindow.Instance.OpenBar!.RenderTransform as TranslateTransform;
            if(transform == null) { 
                transform = new TranslateTransform();
            }

            MainWindow.Instance.OpenBar.IsVisible = true;
            MainWindow.Instance.OpenBar.IsHitTestVisible = true;
            OpacityChangeAnimation animation = new(true);
            TranslateXAnimation animation2 = new(transform.X, 0);
            animation2.RunAnimation(MainWindow.Instance.OpenBar);

            TranslateXAnimation animation1 = new(100, 0);
            animation1.RunAnimation(MainWindow.Instance.ToolBar);
            animation.RunAnimation(MainWindow.Instance.Back);
        }
    }
}
