using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;

namespace WonderLab.ViewModels;

public class ViewModelBase : ObservableObject {
    public Dispatcher Dispatcher => Dispatcher.UIThread;

    protected virtual void RunBackgroundWork(Action action) {
        BackgroundWorker worker = new();
        worker.DoWork += (s, e) => action();
        worker.RunWorkerAsync();
    }
}
