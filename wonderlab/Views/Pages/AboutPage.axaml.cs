using Avalonia.Controls;
using System.Threading.Tasks;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class AboutPage : UserControl {   
        public AboutPage() {
            Initialized += InitializedAction;
            InitializeComponent();
            DataContext = new AboutPageViewModel();
        }

        private async void InitializedAction(object? sender, System.EventArgs e) {       
            await Task.Delay(100);
            TopBar.Margin = new(0);
        }
    }
}
