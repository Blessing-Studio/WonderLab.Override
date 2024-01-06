using Avalonia.Controls;
using WonderLab.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Views.Dialogs;

public partial class UpdateDialogContent : UserControl {
    public UpdateDialogContent() {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetService<UpdateDialogContentViewModel>()!;
    }
}