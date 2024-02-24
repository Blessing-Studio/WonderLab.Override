using System;
using WonderLab.ViewModels;

namespace WonderLab.Classes.Interfaces;

/// <summary>
/// 导航服务统一接口
/// </summary>
public interface INavigationService {
    Action<object>? NavigationRequest { get; set; }

    /// <summary>
    /// 导航方法
    /// </summary>
    void NavigationTo<TViewModel>() where TViewModel : ViewModelBase;
}