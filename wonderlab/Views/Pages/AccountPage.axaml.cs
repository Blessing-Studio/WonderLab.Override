using Avalonia.Controls;
using System.Threading;
using System.Threading.Tasks;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class AccountPage : UserControl
    {
        public static AccountPageViewModel ViewModel { get; private set; } = new();

        public AccountPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            Initialize();
        }

        public async void Initialize() {
            await Task.Delay(300);
            //TopBar.Margin = new(0);
        }
    }
}
