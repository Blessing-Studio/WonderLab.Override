using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;

namespace wonderlab.ViewModels.Pages {
    public class SingleGameCoreConfigPageViewModel : ViewModelBase {
        private GameCoreViewData Current = null!;

        public SingleGameCoreConfigPageViewModel(GameCoreViewData data) {
            PropertyChanged += OnPropertyChanged;
            Current = data;
            SingleConfigEnabledText = data.SingleConfig.IsSingleConfigEnabled ? "启用" : "禁用";
            IsSingleConfigEnabled = data.SingleConfig.IsSingleConfigEnabled;
            IsFullScreen = data.SingleConfig.IsFullScreen;
            FullScreenText = data.SingleConfig.IsFullScreen ? "启用" : "禁用";
        }

        [Reactive]
        public string SingleConfigEnabledText { get; set; }

        [Reactive]
        public string FullScreenText { get; set; }

        [Reactive]
        public bool IsSingleConfigEnabled { get; set; }

        [Reactive]
        public bool IsFullScreen { get; set; }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(IsSingleConfigEnabled)) {
                Current.SingleConfig.IsSingleConfigEnabled = IsSingleConfigEnabled;
                SingleConfigEnabledText = IsSingleConfigEnabled ? "启用" : "禁用";
            } else if (e.PropertyName is nameof(IsFullScreen)) {
                Current.SingleConfig.IsFullScreen = IsFullScreen;
                FullScreenText = IsFullScreen ? "启用" : "禁用";
            }
        }

        public void SingleConfigOpenAction() {
            JsonUtils.SaveSingleGameCoreJson(Current);
        }

        public void FullScreenOpenAction() {
            JsonUtils.SaveSingleGameCoreJson(Current);
        }
    }
}
