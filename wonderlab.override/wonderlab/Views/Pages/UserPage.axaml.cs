using Avalonia.Controls;
using System.Threading;
using System.Threading.Tasks;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class UserPage : UserControl
    {
        //public static UserPage Current { get; private set; }
        public static UserPageViewModel ViewModel { get; private set; } = new();

        public UserPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
