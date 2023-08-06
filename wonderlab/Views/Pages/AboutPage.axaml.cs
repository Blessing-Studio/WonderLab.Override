using Avalonia.Controls;
using System.Threading.Tasks;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class AboutPage : UserControl {   
        public AboutPage() {
            InitializeComponent();
            DataContext = new AboutPageViewModel();
        }
    }
}
