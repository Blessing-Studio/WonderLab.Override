using System;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace WonderLab.Classes.Interfaces;

public interface ITaskJob : INotifyPropertyChanged, IDisposable {
    string JobName { get; set; }
    double Progress { get; set; }
    bool IsDeletedRequested { get; }
    bool CanBeCancelled { get; set; }
    bool IsIndeterminate { get; set; }
    string ProgressDetail { get; set; }
    TaskStatus TaskStatus { get; set; }
    ValueTask? WorkingTask { get; set; }
    IRelayCommand CancelTaskCommand { get; }
    IRelayCommand RequestDeleteCommand { get; }
    CancellationTokenSource CancellationTokenSource { get; }

    void InvokeTaskFinished();
    ValueTask BuildWorkItemAsync(CancellationToken token);
    ValueTask<TaskStatus> WaitForRunAsync(CancellationToken token);

    event EventHandler<EventArgs> TaskFinished;
}
