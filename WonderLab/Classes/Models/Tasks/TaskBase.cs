using System;
using System.Threading;
using Avalonia.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WonderLab.Classes.Models.Tasks {
    public abstract partial class TaskBase : ObservableObject, ITaskJob {
        private bool disposed;

        private bool _isTaskFinishedEventFired;

        private double _pendingProgress;

        private string _pendingDetail;

        [ObservableProperty]
        public string jobName;

        [ObservableProperty]
        public bool isDeletedRequested;

        [ObservableProperty]
        public bool canBeCancelled;

        [ObservableProperty]
        public TaskStatus taskStatus;

        [ObservableProperty]
        public string progressDetail;

        [ObservableProperty]
        public bool isIndeterminate;

        [ObservableProperty]
        public double progress;

        [ObservableProperty]
        public ValueTask? workingTask;

        public CancellationTokenSource CancellationTokenSource => new CancellationTokenSource();

        public event EventHandler<EventArgs>? TaskFinished;

        public static readonly DispatcherTimer RenderRefreshTimer = new(DispatcherPriority.Normal) {
            Interval = TimeSpan.FromSeconds(0.25),
            IsEnabled = true
        };

        public TaskBase() {
            RenderRefreshTimer.Tick += RefreshProgressOnTick;
        }

        public abstract ValueTask BuildWorkItemAsync(CancellationToken token);

        public void Cleanup() {
            RenderRefreshTimer.Tick -= RefreshProgressOnTick;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void InvokeTaskFinished() {
            if (!_isTaskFinishedEventFired) {
                this.TaskFinished?.Invoke(this, EventArgs.Empty);
                _isTaskFinishedEventFired = true;
            }
        }

        public async ValueTask<TaskStatus> WaitForRunAsync(CancellationToken token) {
            await Task.Delay(200, token);
            while (!token.IsCancellationRequested) {
                var taskStatus = TaskStatus;
                if ((uint)(taskStatus - 5) <= 2u) {
                    break;
                }

                await Task.Delay(250, token);
            }

            return TaskStatus;
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

        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                if (disposing) {
                    CancellationTokenSource.Dispose();
                    Cleanup();
                }

                disposed = true;
            }
        }

        protected void ReportProgress(double progress, string? detail = null) {
            _pendingProgress = progress;
            _pendingDetail = detail!;
        }
        
        protected virtual void RefreshProgressOnTick(object? sender, EventArgs e) {
            if (!Equals(Progress, _pendingProgress)) {
                if (double.IsNegative(_pendingProgress) && !IsIndeterminate) {
                    IsIndeterminate = true;
                } else if (_pendingProgress != 0.0) {
                    Progress = _pendingProgress;
                }
            }
            if (!string.IsNullOrEmpty(_pendingDetail)) {
                ProgressDetail = _pendingDetail;
            }
        }
    }
}
