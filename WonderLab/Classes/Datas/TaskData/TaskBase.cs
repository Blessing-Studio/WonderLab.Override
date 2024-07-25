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
    private DispatcherTimer _debounceTimer;
    private const int MinUpdateInterval = 400; // 最小更新间隔（毫秒）
    private DateTime _lastUpdateTime;

    private double _insideProgress;
    private bool _insideIsIndeterminate;
    private string _insideProgressDetail;

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

    public TaskBase() {
        _debounceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        _debounceTimer.Tick += (s, e) => {
            (s as DispatcherTimer).Stop();
            Progress = _insideProgress;
            ProgressDetail = _insideProgressDetail;
        };
    }

    public async ValueTask<TaskStatus> WaitForRunAsync(CancellationToken token) {
        await Task.Delay(200, token);
        while (!token.IsCancellationRequested) {
            if ((uint)(TaskStatus - 5) <= 2u) {
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

    protected bool ShouldUpdate() {
        var now = DateTime.Now;
        if ((now - _lastUpdateTime).TotalMilliseconds >= MinUpdateInterval) {
            _lastUpdateTime = now;
            return true;
        }
        return false;
    }

    protected void DebounceUIUpdate() {
        Progress = _insideProgress;
        ProgressDetail = _insideProgressDetail;
    }

    protected void ReportProgress(string detail) {
        Dispatcher.UIThread.Post(() => {
            if (!string.IsNullOrEmpty(detail)) {
                ProgressDetail = detail;
            }
        });
    }

    protected void ReportProgress(double progress) {
        Dispatcher.UIThread.Post(() => {
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