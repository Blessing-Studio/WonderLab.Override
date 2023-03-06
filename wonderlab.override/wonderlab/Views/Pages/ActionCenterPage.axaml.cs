using Avalonia.Controls;
using System.Threading.Tasks;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class ActionCenterPage : UserControl
    {
        public static ActionCenterPageViewModel ViewModel { get; set; }
        public ActionCenterPage() {    
            InitializeComponent();
            DataContext = ViewModel = new();
        }

        private async void InitializedAction(object? sender, System.EventArgs e) {       
            await Task.Delay(200);
            RunIconAnimation();
        }

        public async void RunIconAnimation() {
            icon1.Height = 35;
            await Task.Delay(100);
            icon2.Height = 25;
            await Task.Delay(100);
            icon3.Height = 15;
        }
    }
}
