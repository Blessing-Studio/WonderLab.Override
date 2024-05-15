using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MinecraftLaunch.Classes.Enums;
using MinecraftLaunch.Classes.Interfaces;
using MinecraftLaunch.Classes.Models.Auth;
using MinecraftLaunch.Components.Authenticator;

namespace WonderLab.Services.Auxiliary;

/// <summary>
/// 游戏账户服务类
/// </summary>
public sealed class AccountService {
    private OfflineAuthenticator _offlineAuthenticator;
    private MicrosoftAuthenticator _microsoftAuthenticator;
    private YggdrasilAuthenticator _yggdrasilAuthenticator;

    /// <summary>
    /// 初始化验证器组件
    /// </summary>
    public bool InitializeComponent<T>(IAuthenticator<T> authenticator, AccountType type = AccountType.Offline) {
        try {
            switch (type) {
                case AccountType.Offline:
                    _offlineAuthenticator = authenticator as OfflineAuthenticator;
                    return true;
                case AccountType.Microsoft:
                    _microsoftAuthenticator = authenticator as MicrosoftAuthenticator;
                    return true;
                case AccountType.Yggdrasil:
                    _yggdrasilAuthenticator = authenticator as YggdrasilAuthenticator;
                    return true;
                case AccountType.UnifiedPass://暂不支持
                default:
                    return false;
            }
        } catch (Exception) {
            return false;
        }
    }

    /// <summary>
    /// 异步验证
    /// </summary>
    public async ValueTask<IEnumerable<Account>> AuthenticateAsync(
        int type, 
        Action<DeviceCodeResponse> action = default, 
        CancellationTokenSource tokenSource = default) {

        switch (type) {
            case 1:
                return Enumerable.Repeat(_offlineAuthenticator.Authenticate(), 1);
            case 2:
                await _microsoftAuthenticator.DeviceFlowAuthAsync(action, tokenSource);
                return Enumerable.Repeat(await _microsoftAuthenticator.AuthenticateAsync(), 1);
            case 3:
                return await _yggdrasilAuthenticator.AuthenticateAsync();
            default:
                throw new Exception("Unkown Account Type");
        }
    }
}