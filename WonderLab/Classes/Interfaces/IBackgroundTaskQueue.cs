using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WonderLab.Classes.Interfaces {
    public interface IBackgroundTaskQueue {
        ValueTask QueueBackgroundWorkItemAsync(ITaskJob job);

        ValueTask<ITaskJob> DequeueAsync(CancellationToken cancellationToken);
    }
}
