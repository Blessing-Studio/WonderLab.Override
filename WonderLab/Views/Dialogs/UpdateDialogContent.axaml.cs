using Avalonia.Controls;
using Avalonia.ReactiveUI;
using WonderLab.ViewModels.Dialogs;

namespace WonderLab.Views.Dialogs {
    public partial class UpdateDialogContent : ReactiveUserControl<UpdateDialogContentViewModel> {
        public UpdateDialogContent() {
            InitializeComponent();
        }

        public UpdateDialogContent(UpdateDialogContentViewModel vm) {
            InitializeComponent();
            ViewModel = vm;
        }
    }
}
