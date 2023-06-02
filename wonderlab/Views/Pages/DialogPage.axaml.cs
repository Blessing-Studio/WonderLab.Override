using Avalonia.Controls;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages {
    public partial class DialogPage : UserControl {
        public static DialogPageViewModel ViewModel { get; set; } = new();

        public DialogPage() {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public void ShowInfoDialog(string title, string message) {
            MainDialog.Title = title;
            MainDialog.Message = message;
            MainDialog.ShowDialog();
        }
    }
}
