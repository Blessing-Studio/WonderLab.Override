using Avalonia.Controls;
using wonderlab.Class.Utils;
using wonderlab.ViewModels.Dialogs;

namespace wonderlab.Views.Dialogs {
    public partial class UpdateDialogContent : UserControl {
        public static UpdateDialogContentViewModel ViewModel { get; set; }  

        public UpdateDialogContent() {
            InitializeComponent();
            DataContext = this;
        }

        public UpdateDialogContent(VersionInfo versionInfo,
            string message, string author) {
            InitializeComponent();
            DataContext = ViewModel = new(versionInfo, message, author);
        }
    }
}
