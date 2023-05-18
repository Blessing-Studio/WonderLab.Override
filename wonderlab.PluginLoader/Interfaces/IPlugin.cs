

namespace wonderlab.PluginLoader.Interfaces
{
    /// <summary>
    /// 插件基类
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 当插件加载时
        /// </summary>
        public void OnLoad();
        /// <summary>
        /// 当插件卸载时
        /// </summary>
        public void OnUnload();
        /// <summary>
        /// 当插件启用时
        /// </summary>
        /// <returns></returns>
        public bool OnEnable();
        /// <summary>
        /// 当插件卸载时
        /// </summary>
        public void OnDisable();
    }
    /// <summary>
    /// 插件拓展方法
    /// </summary>
    public static class PluginExtra
    {
        /// <summary>
        /// 获取插件信息
        /// </summary>
        /// <param name="plugin">插件类</param>
        /// <returns>插件信息</returns>
        public static PluginInfo GetPluginInfo(this IPlugin plugin)
        {
            return PluginLoader.GetPluginInfo(plugin)!;
        }
    }
}
