using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Classes.Datas.MessageData;
using MinecraftLaunch.Classes.Models.Auth;

namespace WonderLab.Classes.Datas.TaskData;
public sealed class AccountLoadTask : TaskBase {
    private IEnumerable<Account> _accounts;

    public AccountLoadTask(IEnumerable<Account> accounts) { 
        _accounts = accounts;
        JobName = "账户信息加载任务";
    }

    public override async ValueTask BuildWorkItemAsync(CancellationToken token) {
        var tasks = _accounts.Select(x => {
            return Task.Run(() => new AccountViewData(x));
        });

        await Task.WhenAll(tasks).ContinueWith(async x => {
            if (x.IsCompletedSuccessfully) {
                WeakReferenceMessenger.Default.Send(new AccountViewMessage(await x));
            }
        });
    }
}