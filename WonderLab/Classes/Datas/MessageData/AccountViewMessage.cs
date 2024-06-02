using System.Collections.Generic;
using WonderLab.Classes.Datas.ViewData;

namespace WonderLab.Classes.Datas.MessageData;

public sealed record AccountViewMessage(IEnumerable<AccountViewData> accounts) {
    public IEnumerable<AccountViewData> Accounts => accounts;
}