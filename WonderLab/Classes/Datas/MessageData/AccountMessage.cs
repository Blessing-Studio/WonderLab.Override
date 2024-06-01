using System.Collections.Generic;
using MinecraftLaunch.Classes.Models.Auth;

namespace WonderLab.Classes.Datas.MessageData;

public sealed class AccountMessage(IEnumerable<Account> accounts) {
    public IEnumerable<Account> Accounts => accounts;
}