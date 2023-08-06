using Avalonia.Controls;
using wonderlab.Class.ViewData;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages {
    public partial class SingleGameCoreConfigPage : UserControl {
        public GameCoreViewData ViewData { get; set; } = null;
        public SingleGameCoreConfigPageViewModel ViewModel { get; set; }
        public SingleGameCoreConfigPage() {
            InitializeComponent();
        }

        public SingleGameCoreConfigPage(GameCoreViewData data) {
            InitializeComponent();
            ViewData = data;
            DataContext = ViewModel = new(data);
        }
    }
}
