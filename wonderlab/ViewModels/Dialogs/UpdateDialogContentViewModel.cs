using DialogHostAvalonia;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using wonderlab.Class.Utils;

namespace wonderlab.ViewModels.Dialogs {
    public class UpdateDialogContentViewModel : ViewModelBase {
        private JsonNode VersionInfo { get; set; }

        public UpdateDialogContentViewModel(JsonNode versionInfo,
            string message, string author) {
            VersionInfo = versionInfo;
            Message = message;
            Author = author;
        }

        [Reactive]
        public string Message { get; set; }

        [Reactive]
        public string Author { get; set; }

        [Reactive]
        public bool Update { get; set; }

        [Reactive]
        public double UpdateProgress { get; set; }

        public void UpdateAction() {
            Update = true;
            UpdateUtils.UpdateAsync(VersionInfo, x => {
                UpdateProgress = x * 100;
            });
        }

        public void CloseAction() {
            DialogHost.Close("dialogHost");
        }
    }
}
