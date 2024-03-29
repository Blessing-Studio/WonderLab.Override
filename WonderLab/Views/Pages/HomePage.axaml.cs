using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Diagnostics;

namespace WonderLab.Views.Pages;

public partial class HomePage : UserControl {
    public HomePage() {
        InitializeComponent();
        Debug.WriteLine("HomePage");
    }
}