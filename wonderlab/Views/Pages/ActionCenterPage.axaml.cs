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
            Bitmap.PointerEntered += (_,_) => {
                Content.Height = 0;
            };

            Bitmap.PointerExited += (_,_) => {
                Content.Height = 50;
            };
        }
    }
}
