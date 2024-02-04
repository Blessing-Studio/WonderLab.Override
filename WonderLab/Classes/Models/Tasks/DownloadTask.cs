using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WonderLab.Classes.Models.Tasks;

public class DownloadTask : TaskBase
{
    public DownloadTask(string uri, DirectoryInfo Directory, string fileName = null)
    {
        JobName = $"文件 {fileName} 的下载任务";
    }

    public override async ValueTask BuildWorkItemAsync(CancellationToken token)
    {
    }
}
