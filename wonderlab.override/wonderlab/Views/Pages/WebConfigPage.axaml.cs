using Avalonia.Controls;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class WebConfigPage : UserControl {   
        public static WebConfigPageViewModel ViewModel { get; set; } = new();
        public WebConfigPage() {       
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
