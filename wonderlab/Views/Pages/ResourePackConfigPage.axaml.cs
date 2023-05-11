using Avalonia.Controls;
using MinecraftLaunch.Modules.Models.Launch;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class ResourePackConfigPage : UserControl
    {
        public static ResourePackConfigPageVM ViewModel { get; set; }
        public ResourePackConfigPage()
        {
            InitializeComponent();
        }

        public ResourePackConfigPage(GameCore core)
        {
            InitializeComponent();
            DataContext = ViewModel = new(core);
        }
    }
}
