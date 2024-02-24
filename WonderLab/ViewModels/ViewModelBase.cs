using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.ViewModels;

public class ViewModelBase : ObservableObject {
    public Dispatcher Dispatcher => Dispatcher.UIThread;
}
