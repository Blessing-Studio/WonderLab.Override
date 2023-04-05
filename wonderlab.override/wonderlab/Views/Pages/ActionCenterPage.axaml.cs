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
            Bitmap.PointerEnter += (_,_) => {
                Content.Height = 0;
            };

            Bitmap.PointerLeave += (_,_) => {
                Content.Height = 50;
            };
        }

        private async void InitializedAction(object? sender, System.EventArgs e) {
            await Task.Delay(100);
            TopBar.Margin = new(0);
        }
    }
}
