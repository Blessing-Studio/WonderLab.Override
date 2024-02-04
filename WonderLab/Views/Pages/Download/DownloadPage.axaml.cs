using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.ViewModels.Pages.Download;

namespace WonderLab.Views.Pages.Download;

public partial class DownloadPage : UserControl
{
    public DownloadPageViewModel ViewModel { get; set; }

    public DownloadPage()
    {
        InitializeComponent();
        DataContext = ViewModel = App.ServiceProvider.GetService<DownloadPageViewModel>();
    }
}
