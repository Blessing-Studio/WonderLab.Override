using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using ReactiveUI;

namespace WonderLab.Classes.Interfaces {
    /// <summary>
    /// 调度任务统一接口
    /// </summary>
    public interface ITaskJob : INotifyPropertyChanged, IDisposable {
        string JobName { get; set; }

        bool IsDeletedRequested { get; }

        bool CanBeCancelled { get; set; }

        TaskStatus TaskStatus { get; set; }

        string ProgressDetail { get; set; }

        bool IsIndeterminate { get; set; }

        double Progress { get; set; }

        ValueTask? WorkingTask { get; set; }

        IReactiveCommand CancelTaskCommand { get; }

        IReactiveCommand RequestDeleteCommand { get; }

        CancellationTokenSource CancellationTokenSource { get; }

        event EventHandler<EventArgs> TaskFinished;

        ValueTask<TaskStatus> WaitForRunAsync(CancellationToken token);

        ValueTask BuildWorkItemAsync(CancellationToken token);

        void Cleanup();

        void InvokeTaskFinished();
    }
}
