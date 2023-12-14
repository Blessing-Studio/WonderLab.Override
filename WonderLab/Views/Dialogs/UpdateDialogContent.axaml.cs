using Avalonia.Controls;
using WonderLab.ViewModels.Dialogs;

namespace WonderLab.Views.Dialogs {
    public partial class UpdateDialogContent : UserControl {
        public UpdateDialogContentViewModel ViewModel { get; set; }

        public UpdateDialogContent() {
            InitializeComponent();
        }

        public UpdateDialogContent(UpdateDialogContentViewModel vm) {
            InitializeComponent();
            DataContext = ViewModel = vm;
        }
    }
}
