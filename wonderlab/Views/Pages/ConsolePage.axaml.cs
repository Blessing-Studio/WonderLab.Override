using Avalonia.Controls;
using wonderlab.Class.ViewData;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class ConsolePage : UserControl {
        public static ConsolePageViewModel ViewModel { get; set; }

        public ConsolePage() {        
            InitializeComponent();
            DataContext = ViewModel;
        }

        public ConsolePage(MinecraftProcessViewData data) {       
            InitializeComponent();
            DataContext = ViewModel = new(data, e);
        }
    }
}
