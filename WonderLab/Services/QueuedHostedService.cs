using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Avalonia.Threading;

namespace WonderLab.Services;

public sealed class QueuedHostedService : BackgroundService {
    private long _currentRunningJobs;

    private readonly Dispatcher _dispatcher;
    private readonly ILogger<QueuedHostedService> _logger;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;

    public QueuedHostedService(Dispatcher dispatcher,IBackgroundTaskQueue backgroundTaskQueue, ILogger<QueuedHostedService> logger) {
        _logger = logger;
        _dispatcher = dispatcher;
        _backgroundTaskQueue = backgroundTaskQueue;
    }

    public override Task StopAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("程序主机已关闭");
        return base.StopAsync(stoppingToken);
    }

    public override Task StartAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("程序主机已开启");
        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        return ProcessTaskQueueAsync(stoppingToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken) {
        Stopwatch sw = new();
        while (!stoppingToken.IsCancellationRequested) {
            if (Interlocked.Read(ref _currentRunningJobs) == 3) {
                await Task.Delay(10, stoppingToken);
                continue;
            }

            ITaskJob workItem = default!;

            try {
                workItem = await _backgroundTaskQueue.DequeueAsync(stoppingToken);
                Interlocked.Increment(ref _currentRunningJobs);
                _logger.LogInformation("成功从任务队列获取任务：{JobName}", workItem.JobName);

                sw.Start();
                workItem.TaskStatus = TaskStatus.Created;
                workItem.TaskStatus = TaskStatus.WaitingForActivation;

                var value = workItem.BuildWorkItemAsync(workItem.CancellationTokenSource.Token);

                workItem.WorkingTask = value;
                workItem.TaskStatus = TaskStatus.WaitingToRun;
                workItem.TaskStatus = TaskStatus.Running;

                await value;

                await _dispatcher.InvokeAsync(() => {
                    workItem.InvokeTaskFinished();
                    workItem.TaskStatus = TaskStatus.RanToCompletion;
                });

                sw.Stop();
                _logger.LogInformation("任务 {JobName} 已完成执行，用时：{TotalSeconds} 秒。", workItem.JobName, sw.Elapsed.TotalSeconds);
            } catch (OperationCanceledException) {
                if (workItem != null) {
                    workItem.TaskStatus = TaskStatus.Canceled;
                    _logger.LogInformation("任务 {workItem.JobName} 被取消了", workItem.JobName);
                }
            } catch (Exception) {
                if (workItem != null) {
                    workItem.TaskStatus = TaskStatus.Faulted;
                }
            } finally {
                Interlocked.Decrement(ref _currentRunningJobs);
                if (workItem != null) {
                    workItem.WorkingTask = null;
                }

                workItem?.Dispose();

                sw.Reset();
            }
        }
    }
}