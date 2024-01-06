using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using WonderLab.Classes.Interfaces;

namespace WonderLab.Services;

public class QueuedHostedService(IBackgroundTaskQueue taskQueue) : BackgroundService {
    private long _currentRunningJobs;
    
    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken) {
        Stopwatch sw = new();
        while (!stoppingToken.IsCancellationRequested) {
            if (Interlocked.Read(ref _currentRunningJobs) == 3) {
                await Task.Delay(10, stoppingToken);
                continue;
            }

            ITaskJob workItem = null;

            try {
                workItem = await taskQueue.DequeueAsync(stoppingToken);
                Interlocked.Increment(ref _currentRunningJobs);
                Debug.WriteLine($"成功从任务队列获取任务 [{workItem.JobName}]。");
                sw.Start();
                workItem.TaskStatus = TaskStatus.Created;
                workItem.TaskStatus = TaskStatus.WaitingForActivation;
                ValueTask value = workItem.BuildWorkItemAsync(workItem.CancellationTokenSource.Token);
                workItem.WorkingTask = value;
                workItem.TaskStatus = TaskStatus.WaitingToRun;
                workItem.TaskStatus = TaskStatus.Running;
                await value;
                workItem.TaskStatus = TaskStatus.RanToCompletion;
                workItem.InvokeTaskFinished();
                sw.Stop();
                Debug.WriteLine($"任务 [{workItem.JobName}] 已完成执行，用时：[{sw.Elapsed.TotalSeconds} 秒]。");
            }
            catch (OperationCanceledException) {
                if (workItem != null) {
                    workItem.TaskStatus = TaskStatus.Canceled;
                    Debug.WriteLine($"任务 [{workItem.JobName}] 被取消了。");
                }
            }
            catch (Exception ex2) {
                if (workItem != null) {
                    workItem.TaskStatus = TaskStatus.Faulted;
                }
            }
            finally {
                Interlocked.Decrement(ref _currentRunningJobs);
                if (workItem != null) {
                    workItem.WorkingTask = null;
                }

                workItem?.Dispose();
                sw.Reset();
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken) {
        Debug.WriteLine("QueuedHostedService is stopping.");
        await base.StopAsync(stoppingToken);
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        return Task.WhenAll(ProcessTaskQueueAsync(stoppingToken), ProcessTaskQueueAsync(stoppingToken),
            ProcessTaskQueueAsync(stoppingToken));
    }
}
