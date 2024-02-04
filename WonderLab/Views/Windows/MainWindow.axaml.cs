using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.ViewModels.Windows;

namespace WonderLab.Views.Windows;

public partial class MainWindow : Window
{
    private MainWindowViewModel ViewModel { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = ViewModel = App.ServiceProvider.GetService<MainWindowViewModel>();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (Design.IsDesignMode)
        {
            return;
        }

        ViewModel.Init();
    }
}
