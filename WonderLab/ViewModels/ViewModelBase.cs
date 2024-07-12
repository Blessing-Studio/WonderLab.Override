using System;
using Avalonia.Threading;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.ViewModels;

public class ViewModelBase : ObservableObject {
    protected virtual void RunBackgroundWork(Action action, Action completed = default) {
        BackgroundWorker worker = new();
        worker.DoWork += (s, e) => action();
        worker.RunWorkerCompleted += (s, e) => completed?.Invoke();

        worker.RunWorkerAsync();
    }
}

public class DialogViewModelBase : ViewModelBase {
    public virtual object Parameter { get; set; }

    public void Initialize(object parameter) {
        Parameter = parameter;
    }
}