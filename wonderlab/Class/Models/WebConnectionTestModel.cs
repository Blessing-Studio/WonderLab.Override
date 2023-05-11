using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Utils;

namespace wonderlab.Class.Models
{
    public class WebConnectionTestModel : ReactiveObject { 
        public string Url{ get; set; }

        [Reactive]
        public bool IsSuccess { get; set; } = false;

        [Reactive]
        public bool IsError { get; set; } = false;

        [Reactive]
        public bool IsLoading { get; set; } = true;
        
        [Reactive]
        public double SuccessBorderWidth { get; set; } = 0;

        [Reactive]
        public double ErrorBorderWidth { get; set; } = 0;

        [Reactive]
        public double LoadingBorderWidth { get; set; } = 120;
        
        [Reactive]
        public string Name { get; set; }

        public WebConnectionTestModel(string url) {
            Url = url;
            Name = Url.Replace("https://", "").Replace("http://", "");
            _ = Run();
        }

        public WebConnectionTestModel(string url, string name) {
            Url = url;
            Name = name;
            _ = Run();
        }

        public async ValueTask Run() {
            await Task.Run(async () => IsSuccess = await HttpUtils.ConnectionTestAsync(Url));
            $"测试返回的状态 {IsSuccess}".ShowLog();

            if (IsSuccess) {
                await Task.Run(async () => {
                    LoadingBorderWidth = 0;
                    await Task.Delay(5);
                    SuccessBorderWidth = 100;
                    await Task.Delay(20);
                    IsLoading = false;
                });
            }
            else {
                IsError = true;
                await Task.Run(async () => {
                    LoadingBorderWidth = 0;
                    await Task.Delay(5);
                    ErrorBorderWidth = 140;
                    await Task.Delay(20);
                    IsLoading = false;
                });
            }
        }
    }
}
