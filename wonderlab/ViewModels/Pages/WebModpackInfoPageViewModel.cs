using Avalonia.Media;
using Avalonia.Media.Imaging;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages {
    public class WebModpackInfoPageViewModel : ViewModelBase {
        public WebModpackInfoPageViewModel() {
            PropertyChanged += OnPropertyChanged;
            Screenshots.Add(new Bitmap("C:\\Users\\w\\Desktop\\总整包\\素材库\\图片素材\\桌面背景\\yuanshen.jpeg"));
            Screenshots.Add(new Bitmap("C:\\Users\\w\\Desktop\\总整包\\素材库\\图片素材\\桌面背景\\RIJTTWLM_85WTQGJS9SQ(4V.jpg"));
            Screenshots.Add(new Bitmap("C:\\Users\\w\\Desktop\\总整包\\素材库\\图片素材\\桌面背景\\R-C.jpg"));
            Screenshots.Add(new Bitmap("C:\\Users\\w\\Desktop\\总整包\\素材库\\图片素材\\桌面背景\\vsbj.jpg"));
            Screenshots.Add(new Bitmap("C:\\Users\\w\\Desktop\\总整包\\素材库\\图片素材\\桌面背景\\VL9KJ`D88L@Y7)@~[G{WJ(T.jpg"));
        }

        public WebModpackInfoPageViewModel(WebModpackViewData data) {
            PropertyChanged += OnPropertyChanged;
            Data = data;

            CurrentResourceVersion = data.Data.Files.First().Key;
            ResourceId = data.Data.ChineseTitle;
            if (data.Data.ScreenshotUrls == null) {
                Screenshots.Add(data.Icon);
            } else {
                _ = GetScreenshotsAsync();
            }

            Categories.AddRange(data.Data.Categories);
            var time = DateTime.Now - data.Data.LastUpdateTime;
            StringBuilder builder = new();
            builder.Append("作者：");
            builder.Append(data.Data.Author);
            builder.Append(" 上次更新：");
            builder.Append(time.Days > 365 ? $"{(time.Days / 365).ToInt32()}年前" : time.Days > 31 ? $"{(time.Days / 31).ToInt32()}个月前" : $"{time.Days}天前");
            builder.Append(" 下载量：");
            builder.Append((data.Data.DownloadCount > 1000) ? $"{data.Data.DownloadCount / 1000}k" : data.Data.DownloadCount.ToString());

            ResourceProfile = builder.ToString();
            Icon = data.Icon;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(CurrentResourceVersion)) {
                CurrentResources = Data.Data.Files[CurrentResourceVersion];
            }
        }

        public async ValueTask GetScreenshotsAsync() {
            foreach (var url in Data.Data.ScreenshotUrls.AsParallel()) {
                var result = await Task.Run(async () => {
                    return await HttpUtils.GetWebBitmapAsync(url);
                });

                $"{url} 已成功获取".ShowLog();
                Screenshots.Add(result);
            }
        }

        public override void GoBackAction() {
            new DownCenterPage().Navigation();
        }

        [Reactive]
        public ObservableCollection<IImage> Screenshots { get; set; } = new();

        [Reactive]
        public string ResourceProfile { get; set; } = "作者：* 上次更新：* 下载量：*";

        [Reactive]
        public string ResourceId { get; set; } = "ResourceId";

        [Reactive]
        public ObservableCollection<string> Categories { get; set; } = new();

        [Reactive]
        public Bitmap Icon { get; set; }

        [Reactive]
        public WebModpackViewData Data { get; set; }

        [Reactive]
        public string CurrentResourceVersion { get; set; }

        [Reactive]
        public ObservableCollection<WebModpackFilesModel> CurrentResources { get; set; }
    }
}
