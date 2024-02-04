using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.ViewModels.Dialogs;

namespace WonderLab.Views.Dialogs;

public partial class UpdateDialogContent : UserControl
{
    public UpdateDialogContent()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetService<UpdateDialogContentViewModel>()!;
    }
}