using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.control.Animation;
using wonderlab.control.Controls.Bar;
using wonderlab.Views.Pages;
using wonderlab.Views.Windows;

namespace wonderlab.ViewModels.Pages {
    public class ActionCenterPageViewModel : ViewModelBase {
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

        public async void GetMojangNewsAction()
        {
            try
            {
                New result = null;
                if (CacheResources.MojangNews.Count <= 0)
                {
                    List<New> news = new(await HttpUtils.GetMojangNewsAsync());
                    result = news.Count > 0 ? news.First() : new();
                }
                else
                {
                    result = CacheResources.MojangNews.FirstOrDefault()!;
                }

                NewTitle = result.Title;
                NewTag = result.Tag;
                if (result.NewsPageImage != null)
                {
                    NewImage = await HttpUtils.GetWebBitmapAsync($"https://launchercontent.mojang.com/{result.NewsPageImage.Url}");
                }
            }
            catch (HttpRequestException ex)
            {
                $"哎哟，获取失败力，请检查您的网络是否正常，详细信息：{ex.Message}".ShowMessage();
            }
        }

        public async void GetHitokotoAction() {
            var result = await HttpUtils.GetHitokotoTextAsync();

            if (result != null) {
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
            //App.CurrentWindow.DialogHost.Install.InstallDialog.ShowDialog();
        }

        public void OpenSelectConfigPageAction() {
            new SelectConfigPage().Navigation();
        }

        public void OpenUserPageAction() {
            new AccountPage().Navigation();
        }

        public void OpenServerFindPageAction() {
            new ServerFindPage().Navigation();
        }
    }
}