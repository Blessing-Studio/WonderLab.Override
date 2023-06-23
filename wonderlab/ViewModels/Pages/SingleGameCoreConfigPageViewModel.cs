using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.ViewData;

namespace wonderlab.ViewModels.Pages {
    public class SingleGameCoreConfigPageViewModel : ViewModelBase {
        private GameCoreViewData Current = null!;

        public SingleGameCoreConfigPageViewModel(GameCoreViewData data) {
            PropertyChanged += OnPropertyChanged;
            Current = data;
        }

        [Reactive]
        public bool IsSingleConfigEnabled { get; set; }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            //if (e.PropertyName is nameof(IsSingleConfigEnabled)) {
            //    Current.
            //}else if () {

            //}
        }
    }
}
