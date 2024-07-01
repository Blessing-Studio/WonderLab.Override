using System.Collections.Generic;
using MinecraftLaunch.Classes.Models.Auth;

namespace WonderLab.Classes.Datas.MessageData;

public sealed record AccountMessage(IEnumerable<Account> Accounts);