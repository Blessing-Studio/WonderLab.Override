using Avalonia.Controls;
using wonderlab.Class.ViewData;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class WebModpackInfoPage : UserControl
    {
        public static WebModpackInfoPageViewModel ViewModel { get; set; }
        public WebModpackInfoPage() {       
            InitializeComponent();
            DataContext = ViewModel = new();
        }

        public WebModpackInfoPage(WebModpackViewData data) {
            InitializeComponent();
            DataContext = ViewModel = new(data);
        }
    }
}
