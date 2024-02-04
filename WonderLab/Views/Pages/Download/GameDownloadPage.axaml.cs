using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.ViewModels.Pages.Download;

namespace WonderLab.Views.Pages.Download;

public partial class GameDownloadPage : UserControl
{
    public GameDownloadPageViewModel ViewModel { get; set; }

    public GameDownloadPage()
    {
        InitializeComponent();
        DataContext = ViewModel = App.ServiceProvider.GetService<GameDownloadPageViewModel>();
    }
}
