using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLoader
{
    /// <summary>
    /// 监听器基类
    /// </summary>
    public interface IListener
    {
        public void Register()
        {
            Event.RegListener(this);
        }
        public PluginInfo PluginInfo { get; set; }
        public void GetEvent(Event @event) {}
    }

}

