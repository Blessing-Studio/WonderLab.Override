using System.Threading;
using System.Threading.Tasks;

namespace WonderLab.Classes.Interfaces;

public interface IBackgroundNotificationQueue {
    ValueTask QueueBackgroundWorkItemAsync(INotification job);
    ValueTask<INotification> DequeueAsync(CancellationToken cancellationToken);
}