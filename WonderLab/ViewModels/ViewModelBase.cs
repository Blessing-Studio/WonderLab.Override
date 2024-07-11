using System;
using Avalonia.Threading;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.ViewModels;

public class ViewModelBase : ObservableObject {
    protected virtual void RunBackgroundWork(Action action) {
        BackgroundWorker worker = new();
        worker.DoWork += (s, e) => action();
        worker.RunWorkerAsync();
    }
}

public class DialogViewModelBase : ViewModelBase {
    public virtual object Parameter { get; set; }

    public void Initialize(object parameter) {
        Parameter = parameter;
    }
}