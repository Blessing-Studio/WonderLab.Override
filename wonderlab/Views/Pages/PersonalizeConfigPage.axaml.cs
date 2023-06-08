using Avalonia.Controls;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages {
    public partial class PersonalizeConfigPage : UserControl {
        public static PersonalizeConfigPageViewModel ViewModel { get; set; } = new();
        public PersonalizeConfigPage() {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
