using Avalonia.Controls;
using wonderlab.ViewModels.Dialogs;

namespace wonderlab.Views.Dialogs {
    public partial class ValidationDialogContent : UserControl {
        public static ValidationDialogContentViewModel ViewModel { get; private set; }

        public ValidationDialogContent() {
            InitializeComponent();
            DataContext = ViewModel = new();
        }
    }
}
