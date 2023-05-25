using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            GetHitokotoAction();
            GetLatestGameCoreAction();
        }

        public void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {       

        }

        
        [Reactive]
        public string? NewTitle { get; set; } = "Loading...";

        [Reactive]
        public string? NewTag { get; set; } = "Loading...";

        [Reactive]
        public string? HitokotoTitle { get; set; }

        [Reactive]
        public string LatestGameCore { get; set; }

        [Reactive]
        public string? HitokotoCreator { get; set; }

        [Reactive]
        public Bitmap NewImage { get; set; }

        public async void GetMojangNewsAction() {
            try {           
                var result = (await HttpUtils.GetMojangNewsAsync()).First();

                if (result != null) {               
                    NewTitle = result.Title;
                    NewTag = result.Tag;

                    NewImage = await HttpUtils.GetWebBitmapAsync($"https://launchercontent.mojang.com/{result.NewsPageImage.Url}");
                }
            }
            catch (Exception) {           
                
            }
        }

        public async void GetHitokotoAction() {
            var result = await HttpUtils.GetHitokotoTextAsync();

            if(result != null) { 
                HitokotoCreator ??= $"-- {result.Creator?.Trim()}";
                HitokotoTitle ??= result.Text ?? "焯";
            }
        }

        public async void GetLatestGameCoreAction() {
            try {
                var result = await HttpUtils.GetLatestGameCoreAsync();
                LatestGameCore = result;
            }
            catch (Exception ex) {           
                $"焯，{ex}".ShowLog();
            }
        }

        public void OpenInstallDialogAction() {
            new DownCenterPage().Navigation();
            //MainWindow.Instance.Install.InstallDialog.ShowDialog();
        }

        public void OpenSelectConfigPageAction() {
            new SelectConfigPage().Navigation();
        }

        public void OpenUserPageAction() { 
            new UserPage().Navigation();
        }

        public void OpenServerFindPageAction() {
            new ServerFindPage().Navigation();
        }

        public async void ReturnAction() {
            Dispatcher.UIThread.Post(() => {           
                MainWindow.Instance.OpenTopBar();
                new HomePage().Navigation();
                OpacityChangeAnimation animation = new(true);
                animation.RunAnimation(MainWindow.Instance.Back);
            });
        }
    }
}