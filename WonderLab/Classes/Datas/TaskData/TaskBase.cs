using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;

namespace WonderLab.Classes.Datas.TaskData;

/// <summary>
/// 任务基类
/// </summary>
public abstract partial class TaskBase : ObservableObject, ITaskJob, IDisposable {
    private bool _isTaskFinishedEventFired;

    [ObservableProperty] private double _progress;

    [ObservableProperty] private string _jobName;
    [ObservableProperty] private string _progressDetail;

    [ObservableProperty] private bool _canBeCancelled;
    [ObservableProperty] private bool _isIndeterminate;
    [ObservableProperty] private bool _isDeletedRequested;

    [ObservableProperty] private TaskStatus _taskStatus;
    [ObservableProperty] private ValueTask? _workingTask;

    public CancellationTokenSource CancellationTokenSource => new();

    public event EventHandler<EventArgs> TaskFinished;

    public async ValueTask<TaskStatus> WaitForRunAsync(CancellationToken token) {
        await Task.Delay(200, token);
        while (!token.IsCancellationRequested) {
            TaskStatus taskStatus = TaskStatus;
            if ((uint)(taskStatus - 5) <= 2u) {
                break;
            }

            await Task.Delay(250, token);
        }

        return TaskStatus;
    }

    public void InvokeTaskFinished() {
        if (!_isTaskFinishedEventFired) {
            this.TaskFinished?.Invoke(this, EventArgs.Empty);
            _isTaskFinishedEventFired = true;
        }
    }

    public abstract ValueTask BuildWorkItemAsync(CancellationToken token);

    public virtual void Dispose() {
        CancellationTokenSource.Dispose();
    }

    [RelayCommand(CanExecute = nameof(CanCancelTask))]
    private void CancelTask() {
        CanBeCancelled = false;
        TaskStatus = TaskStatus.Canceled;
        CancellationTokenSource?.Cancel();
    }

    [RelayCommand(CanExecute = nameof(CanDeleteTask))]
    private void RequestDelete() {
        IsDeletedRequested = true;
        CanBeCancelled = false;
    }

    private bool CanCancelTask() {
        return CanBeCancelled;
    }

    private bool CanDeleteTask() {
        return !IsDeletedRequested;
    }

    protected void ReportProgress(string detail) {
        Dispatcher.UIThread.Post(delegate {
            if (!string.IsNullOrEmpty(detail)) {
                ProgressDetail = detail;
            }
        });
    }

    protected void ReportProgress(double progress) {
        Dispatcher.UIThread.Post(delegate {
            if (progress < 0.0) {
                IsIndeterminate = true;
            }
            Progress = progress;
        });
    }

    protected void ReportProgress(double progress, string detail) {
        ReportProgress(progress);
        ReportProgress(detail);
    }
}