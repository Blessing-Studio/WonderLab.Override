using Avalonia.Controls;
using System.Text.Json.Nodes;
using wonderlab.Class.Utils;
using wonderlab.ViewModels.Dialogs;

namespace wonderlab.Views.Dialogs {
    public partial class UpdateDialogContent : UserControl {
        public static UpdateDialogContentViewModel ViewModel { get; set; }  

        public UpdateDialogContent() {
            InitializeComponent();
            DataContext = this;
        }

        public UpdateDialogContent(JsonNode versionInfo,
            string message, string author) {
            InitializeComponent();
            DataContext = ViewModel = new(versionInfo, message, author);
        }
    }
}
