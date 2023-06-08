using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.PluginLoader.Events;

namespace wonderlab.PluginLoader.Interfaces
{
    /// <summary>
    /// 监听器基类
    /// </summary>
    public interface IListener
    {
        /// <summary>
        /// 注册监听器
        /// </summary>
        public void Register()
        {
            Event.RegListener(this);
        }
        /// <summary>
        /// 插件信息
        /// </summary>
        public PluginInfo PluginInfo { get; set; }
        /// <summary>
        /// 全局事件接收函数
        /// </summary>
        /// <param name="event">事件</param>
        public void GetEvent(Event @event) { }
    }

}

