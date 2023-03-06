using Avalonia.Controls;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class NotificationCenterPage : UserControl
    {
        public static NotificationCenterPageViewModel ViewModel { get; set; }
        public NotificationCenterPage()
        {
            InitializeComponent();
            DataContext = ViewModel = new();
        }

        public void Add() { 
            
        }
    }
}
