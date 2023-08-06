using Avalonia.Controls;
using wonderlab.ViewModels.Dialogs;

namespace wonderlab.Views.Dialogs
{
    public partial class GameCrashInfoDialog : UserControl {
        public static GameCrashInfoDialogViewModel? ViewModel { get; set; } = new();
        public GameCrashInfoDialog() {       
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
