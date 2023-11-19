using System;
using ReactiveUI;
using System.Threading;
using Avalonia.Threading;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;
using System.Threading.Channels;
using WonderLab.Classes.Interfaces;
using System.Collections.ObjectModel;

namespace WonderLab.Classes.Managers {
    /// <summary>
    /// 调度任务管理器
    /// </summary>
    public class TaskManager : ReactiveObject {
        private IBackgroundTaskQueue _taskQueue = null!;

        private int _currentRunningJobs;

        [Reactive]
        public ObservableCollection<ITaskJob> TaskJobs { get; set; } = new();

        public TaskManager(IBackgroundTaskQueue queue) {
            _taskQueue = queue;
        }

        public void QueueJob(ITaskJob job) {
            if (job is null) {
                return;
            }

            Task.Run(async () => {
                await _taskQueue.QueueBackgroundWorkItemAsync(job);
                job.TaskFinished += (_, args) => {
                    Interlocked.Decrement(ref _currentRunningJobs);
                    TaskJobs.Remove(job);
                };

                await Dispatcher.UIThread.InvokeAsync(() => {
                    TaskJobs.Add(job);
                });

                Interlocked.Increment(ref _currentRunningJobs);
            });
        }
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue {
        private readonly Channel<ITaskJob> _queue;

        public BackgroundTaskQueue(int queueLength) {
            BoundedChannelOptions boundedChannelOptions = new(queueLength) {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<ITaskJob>(boundedChannelOptions);
        }

        public async ValueTask QueueBackgroundWorkItemAsync(ITaskJob? job) {
            if (job == null) {
                throw new ArgumentNullException(nameof(job));
            }

            await _queue.Writer.WriteAsync(job);
        }

        public async ValueTask<ITaskJob> DequeueAsync(CancellationToken cancellationToken) {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
