using System.Threading;
using System.Threading.Tasks;

namespace WonderLab.Classes.Datas.TaskData;

/// <summary>
/// 游戏启动任务
/// </summary>
public sealed class LaunchTask : TaskBase {
    public override ValueTask BuildWorkItemAsync(CancellationToken token) {
        throw new System.NotImplementedException();
    }
}
