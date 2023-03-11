using Avalonia.Controls;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class SelectConfigPage : UserControl {
        public static SelectConfigPageViewModel ViewModel { get; set; } = new();

        public SelectConfigPage() {       
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
