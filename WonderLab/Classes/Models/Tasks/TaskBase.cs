using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;

namespace WonderLab.Classes.Models.Tasks {
    public abstract class TaskBase : ReactiveObject, ITaskJob {
        private bool disposed;

        private bool _isTaskFinishedEventFired;

        private double _pendingProgress;

        private string _pendingDetail;
        
        [Reactive]
        public string JobName { get; set; }

        [Reactive]
        public bool IsDeletedRequested { get; set; }

        [Reactive]
        public bool CanBeCancelled { get; set; }

        [Reactive]
        public TaskStatus TaskStatus { get; set; }

        [Reactive]
        public string ProgressDetail { get; set; }

        [Reactive]
        public bool IsIndeterminate { get; set; }

        [Reactive]
        public double Progress { get; set; }

        [Reactive]
        public ValueTask? WorkingTask { get; set; }

        public IReactiveCommand CancelTaskCommand => ReactiveCommand.Create(CancelTask);

        public IReactiveCommand RequestDeleteCommand => ReactiveCommand.Create(RequestDelete);

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

        private void CancelTask() {
            CanBeCancelled = false;
            TaskStatus = TaskStatus.Canceled;
            CancellationTokenSource?.Cancel();
        }

        private void RequestDelete() {
            IsDeletedRequested = true;
            CanBeCancelled = false;
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
