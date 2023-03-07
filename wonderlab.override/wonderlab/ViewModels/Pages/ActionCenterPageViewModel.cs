using Avalonia.Media;
using Avalonia.Media.Imaging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Utils;
using wonderlab.control.Animation;
using wonderlab.control.Controls.Bar;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages
{
    public class ActionCenterPageViewModel : ReactiveObject
    {
        public ActionCenterPageViewModel() {
            PropertyChanged += OnPropertyChanged;
            GetMojangNewsAction();
        }

        public void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {       

        }

        
        [Obsolete]//暂时放弃
        public bool IsNewsLoadOk { get; set; }

        [Reactive]
        public string? NewTitle { get; set; } = "Loading...";

        [Reactive]
        public string? NewTag { get; set; } = "Loading...";

        [Reactive]
        public Bitmap NewImage { get; set; }

        public async void GetMojangNewsAction() {
            var result = (await HttpUtils.GetMojangNewsAsync()).First();

            if (result != null) { 
                NewTitle = result.Title;
                NewTag = result.Tag;

                NewImage = await HttpUtils.GetWebBitmapAsync($"https://launchercontent.mojang.com/{result.NewsPageImage.Url}");
            }
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
