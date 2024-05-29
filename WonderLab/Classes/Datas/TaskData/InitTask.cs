using System;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;

using Timer = System.Timers.Timer;

namespace WonderLab.Classes.Datas.TaskData;

public sealed class InitTask(ElapsedEventHandler timerEvent) : TaskBase {
    private readonly Timer _timer = new(TimeSpan.FromMinutes(1));

    public void InitTimer() {
        _timer.Elapsed += timerEvent;
        _timer.Start();
    }

    public void CheckIsTestUser() {

    }

    public async override ValueTask BuildWorkItemAsync(CancellationToken token) {
        InitTimer();
    }
}
