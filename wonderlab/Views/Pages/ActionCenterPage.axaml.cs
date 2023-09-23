using Avalonia.Controls;
using Avalonia.Threading;
using System.Threading.Tasks;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class ActionCenterPage : UserControl
    {
        public static ActionCenterPageViewModel ViewModel { get; set; }
        public ActionCenterPage() {    
            InitializeComponent();
            Loaded += ActionCenterPageLoaded;
            Bitmap.PointerEntered += (_,_) => {
                Content.Height = 0;
            };

            Bitmap.PointerExited += (_,_) => {
                Content.Height = 50;
            };
        }

        private void ActionCenterPageLoaded(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
            if(ViewModel != null) {
                DataContext = ViewModel;
            } else {
                Dispatcher.UIThread.Post(() => {
                    DataContext = ViewModel = new();
                }, DispatcherPriority.Background);
            }
        }
    }
}
