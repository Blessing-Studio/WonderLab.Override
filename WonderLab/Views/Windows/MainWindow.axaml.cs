using Avalonia.Controls;
using Avalonia.Interactivity;
using WonderLab.ViewModels.Windows;

namespace WonderLab.Views.Windows;

public partial class MainWindow : Window {
    public MainWindowViewModel ViewModel { get; set; }

    public MainWindow(MainWindowViewModel vm) {
        InitializeComponent();
        DataContext = ViewModel = vm;
    }

    public MainWindow() {
        InitializeComponent(); 
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (Design.IsDesignMode) {
            return;
        }

        ViewModel.Init();
    }
}
