using System.Threading;
using System.Threading.Tasks;

namespace WonderLab.Classes.Interfaces;

public interface IBackgroundTaskQueue
{
    ValueTask QueueBackgroundWorkItemAsync(ITaskJob job);

    ValueTask<ITaskJob> DequeueAsync(CancellationToken cancellationToken);
}
