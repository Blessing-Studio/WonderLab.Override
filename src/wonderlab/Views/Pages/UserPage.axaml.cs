using Avalonia.Controls;
using System.Threading;
using System.Threading.Tasks;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class UserPage : UserControl
    {
        public static UserPageViewModel ViewModel { get; private set; } = new();

        public UserPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            Initialize();
        }

        public async void Initialize() {
            await Task.Delay(300);
            TopBar.Margin = new(0);
        }
    }
}
