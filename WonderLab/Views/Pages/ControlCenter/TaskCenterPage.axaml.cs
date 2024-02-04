using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.ViewModels.Pages.ControlCenter;

namespace WonderLab.Views.Pages.ControlCenter;

public partial class TaskCenterPage : UserControl
{
    public TaskCenterPageViewModel ViewModel { get; set; }

    public TaskCenterPage()
    {
        InitializeComponent();
        DataContext = ViewModel = App.ServiceProvider.GetService<TaskCenterPageViewModel>();
    }
}
