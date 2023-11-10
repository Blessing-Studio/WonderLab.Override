using Avalonia.Controls;
using Avalonia.Threading;
using System.Threading.Tasks;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class ActionCenterPage : UserControl
    {
        public static ActionCenterPageViewModel ViewModel { get; set; } = new();
        public ActionCenterPage() {    
            InitializeComponent();
            Loaded += ActionCenterPageLoaded;
        }

        private async void ActionCenterPageLoaded(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
            DataContext = ViewModel;

            await Task.Delay(800);
            await Task.Run(() => {
                ViewModel.GetHitokotoAction();
                ViewModel.GetLatestGameCoreAction();
                ViewModel.GetMojangNewsAction();
            });
        }
    }
}
