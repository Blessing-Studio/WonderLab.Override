using Avalonia.Controls;
using System.Diagnostics;
using System.Threading.Tasks;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class NotificationCenterPage : UserControl {   
        public static NotificationCenterPageViewModel ViewModel { get; set; }

        public NotificationCenterPage() {       
            InitializeComponent();
            DataContext = ViewModel = new();

            background.PointerPressed += (_, _) => Close();
        }

        public void Add() { 
            
        }

        public async void Close() {
            background.Opacity = 0;

            Title.Margin = new(0, 0, -140, 0);
            await Task.Delay(50);
            List.Margin = new(0, 0, -880, 0);
            await Task.Delay(50);
            IsHitTestVisible = false;
        }

        public async void Open() {
            IsHitTestVisible = true;
            background.Opacity = 1;
            Title.Margin = new(0);
            await Task.Delay(50);
            List.Margin = new(0);
        }
    }
}
