using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Threading.Channels;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;
using Avalonia.Threading;
using WonderLab.Classes.Datas.ViewData;
using System.Diagnostics;

namespace WonderLab.Services;

public sealed partial class NotificationService(IBackgroundNotificationQueue queue) : ObservableObject {
    private readonly IBackgroundNotificationQueue _taskQueue = queue;

    [ObservableProperty] private ObservableCollection<INotification> notifications = [];

    public void QueueJob(INotification job) {
        if (job is null) {
            return;
        }

        _ = Task.Run(async () => {
            await Dispatcher.UIThread.InvokeAsync(() => {
                Notifications.Add(job);
            });

            (job as NotificationViewData).CloseButtonAction = async () => {
                job.IsCardOpen = true;
                await Task.Delay(TimeSpan.FromSeconds(0.35d)).ContinueWith(x => {
                    Notifications.Remove(job);
                });
            };

            await Task.Delay(4000);
            if (!job.IsCardOpen) {
                job.IsCardOpen = true;
                await Task.Delay(TimeSpan.FromSeconds(0.35d)).ContinueWith(x => {
                    Notifications.Remove(job);
                });
            }

            await _taskQueue.QueueBackgroundWorkItemAsync(job);
        });
    }
}

public sealed class BackgroundNotificationQueue : IBackgroundNotificationQueue {
    private readonly Channel<INotification> _queue;

    public BackgroundNotificationQueue(int queueLength) {
        BoundedChannelOptions boundedChannelOptions = new(queueLength) {
            FullMode = BoundedChannelFullMode.Wait
        };

        _queue = Channel.CreateBounded<INotification>(boundedChannelOptions);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(INotification job) {
        if (job == null) {
            throw new ArgumentNullException(nameof(job));
        }

        await _queue.Writer.WriteAsync(job);
    }

    public async ValueTask<INotification> DequeueAsync(CancellationToken cancellationToken) {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}