using System;
using WonderLab.Classes.Datas;
using WonderLab.ViewModels;

namespace WonderLab.Classes.Interfaces;

/// <summary>
/// 导航服务统一接口
/// </summary>
public interface INavigationService {
    /// <summary>
    /// 导航后执行请求
    /// </summary>
    Action<NavigationPageData> NavigationRequest { get; set; }

    /// <summary>
    /// 导航方法
    /// </summary>
    void NavigationTo<TViewModel>() where TViewModel : ViewModelBase;
}