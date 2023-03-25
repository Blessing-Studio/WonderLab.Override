using Avalonia.Controls;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class GameCoreConfigPage : UserControl
    {
        public static GameCoreConfigPageViewModel ViewModel { get; set; } = new();
        public GameCoreConfigPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
