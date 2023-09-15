using Avalonia.Controls;
using wonderlab.ViewModels.Dialogs;

namespace wonderlab.Views.Dialogs {
    public partial class AccountDialogContent : UserControl {
        public static AccountDialogContentViewModel ViewModel { get; private set; }

        public AccountDialogContent() {
            InitializeComponent();
            DataContext = ViewModel = new();
        }
    }
}
