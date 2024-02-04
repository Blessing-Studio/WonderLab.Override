using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.ViewModels.Pages;

namespace WonderLab.Views.Pages;

public partial class HomePage : UserControl
{
    public HomePageViewModel ViewModel { get; set; }

    public HomePage()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetService<HomePageViewModel>();
    }
}
