using System.Reflection;
using System.Resources;
using wonderlab.PluginLoader.Events;
using wonderlab.PluginLoader.Attributes;
using wonderlab.PluginLoader.Interfaces;
namespace wonderlab.PluginLoader
{
    /// <summary>
    /// 插件加载器
    /// </summary>
    public static class PluginLoader {
        /// <summary>
        /// 插件默认路径
        /// </summary>
        public static string PluginPath = StringUtil.GetSubPath(Environment.CurrentDirectory, "Plugins");
        /// <summary>
        /// 全局插件列表
        /// </summary>
        public static List<PluginInfo> PluginInfos = new List<PluginInfo>();
        /// <summary>
        /// 全局插件类
        /// </summary>
        public static List<IPlugin> Plugins = new List<IPlugin>();
        /// <summary>
        /// 获取插件信息
        /// </summary>
        /// <param name="type">
        /// 需要获取的插件主类
        /// </param>
        /// <returns>
        /// 插件主类对应的插件信息
        /// </returns>
        public static PluginInfo? GetPluginInfo(Type type) {
            Attribute? attribute = Attribute.GetCustomAttribute(type, typeof(PluginAttribute));
            PluginAttribute handler;
            if (attribute != null) {
                handler = (PluginAttribute)attribute;
            } else { return null; }
            if (handler != null) {
                PluginInfo info = new PluginInfo(type);
                info.Name = handler.Name;
                info.Description = handler.Description;
                info.Version = handler.Version;
                info.Guid = handler.Guid;
                info.Path = type.Assembly.Location;
                info.Icon = handler.Icon;
                return info;
            }
            return null;
        }
        /// <summary>
        /// 获取插件信息
        /// </summary>
        /// <param name="Plugin">插件实例</param>
        /// <returns>插件信息</returns>
        public static PluginInfo? GetPluginInfo(IPlugin Plugin) {
            Type type = Plugin.GetType();
            Attribute? attribute = Attribute.GetCustomAttribute(type, typeof(PluginAttribute));
            PluginAttribute handler;
            if (attribute != null) {
                handler = (PluginAttribute)attribute;
            } else { return null; }
            if (handler != null) {
                PluginInfo info = new PluginInfo(type);
                info.Name = handler.Name;
                info.Description = handler.Description;
                info.Version = handler.Version;
                info.Guid = handler.Guid;
                info.Path = type.Assembly.Location;
                info.Icon = handler.Icon;
                info.Author = handler.Author;
                return info;
            }
            return null;

        }
        /// <summary>
        /// 加载插件
        /// </summary>
        /// <param name="Path">
        /// 插件文件路径
        /// </param>
        public static void Load(string Path) {
            Type type = GetMainPluginType(Path);
            PluginInfo? plugin = ((IPlugin)Activator.CreateInstance(type, Array.Empty<object>())!).GetPluginInfo();
            if (plugin == null) {
                return;
            }
            foreach (PluginInfo i in PluginInfos) {
                if (i.Guid == plugin.Guid || i.Name == plugin.Name) {
                    return;
                }
            }
            foreach (Type t in type.Assembly.GetTypes()) {
                if (t.GetCustomAttribute<ListenerAttribute>() != null) {
                    IListener listener = (IListener)Activator.CreateInstance(t)!;
                }
            }
            if (type != null) {
                try {
                    object obj = Activator.CreateInstance(type)!;
                    ((IPlugin)obj).OnLoad();
                    PluginInfo pluginInfo = GetPluginInfo(type)!;
                    pluginInfo.Path = Path;
                    pluginInfo.MainType = type;
                    PluginInfos.Add(pluginInfo);
                    Plugins.Add((IPlugin)obj);
                    PluginLoadEvent e = new PluginLoadEvent();
                    e.PluginInfo = pluginInfo;
                    Event.CallEvent(e);
                }
                catch { }
            }

        }
        /// <summary>
        /// 通关插件名获取插件实例
        /// </summary>
        /// <param name="pluginName">插件名</param>
        /// <returns>插件实例</returns>
        public static IPlugin? GetPlugin(string pluginName) {
            foreach (IPlugin plugin in Plugins) {
                if (plugin.GetPluginInfo().Name == pluginName) {
                    return plugin;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取所有加载的插件
        /// </summary>
        /// <returns>加载的插件</returns>
        public static IPlugin[] GetPlugins() {
            return Plugins.ToArray();
        }
        /// <summary>
        /// 加载插件文件获取插件主类
        /// </summary>
        /// <param name="Path">插件路径</param>
        /// <returns>主类</returns>
        public static Type GetMainPluginType(string Path) {
            Assembly dllFromPlugin = Assembly.LoadFile(Path);
            bool IsPlugin = false;
            string MainClassLocation = string.Empty;
            try {
                string name = System.IO.Path.GetDirectoryName(Path)!;
                ResourceManager resourceManager = new ResourceManager(name + ".Properties.Resources", dllFromPlugin);
                MainClassLocation = resourceManager.GetString("MainClass")!;
            }
            catch { }
            if (MainClassLocation != string.Empty) {
                IsPlugin = true;
            } else {
                foreach (Type t in dllFromPlugin.GetTypes()) {
                    if (t.GetCustomAttribute<PluginAttribute>() != null) {
                        IsPlugin = true;
                        MainClassLocation = t.FullName!;
                        break;
                    }
                }
            }
            if (!IsPlugin) {
                throw new Exception();
            }
            Type type = dllFromPlugin.GetType(MainClassLocation)!;
            return type;
        }
        /// <summary>
        /// 卸载插件
        /// </summary>
        /// <param name="Path">
        /// 已加载插件路径
        /// </param>
        public static void UnLoad(string Path) {
            for (int i = 0; i < PluginInfos.Count; i++) {
                if (PluginInfos[i].Path == Path) {
                    Type type = PluginInfos[i].MainType;
                    if (type != null) {
                        try {
                            object obj = GetPlugin(PluginInfos[i].Name)!;
                            ((IPlugin)obj).OnUnload();
                        }
                        catch (Exception) { }
                    }
                    PluginUnLoadEvent e = new PluginUnLoadEvent();
                    e.PluginInfo = PluginInfos[i];
                    Event.CallEvent(e);
                    for (int j = 0; j < Event.Listeners.Count; j++) {
                        if (Event.Listeners[j].PluginInfo.Guid == PluginInfos[i].Guid) {

                            Event.Listeners.RemoveAt(j);
                            j--;
                        }
                    }
                    Plugins.Remove(GetPlugin(PluginInfos[i].Name)!);
                    PluginInfos.RemoveAt(i);
                    return;
                }
            }
        }
        /// <summary>
        /// 卸载插件
        /// </summary>
        /// <param name="plugin">插件实例</param>
        public static void UnLoad(IPlugin plugin) {
            for (int i = 0; i < PluginInfos.Count; i++) {
                if (PluginInfos[i].Guid == plugin.GetPluginInfo().Guid) {
                    Type type = PluginInfos[i].MainType;
                    if (type != null) {
                        try {
                            plugin.OnUnload();
                        }
                        catch (Exception) { }
                    }
                    PluginUnLoadEvent e = new PluginUnLoadEvent();
                    e.PluginInfo = PluginInfos[i];
                    Event.CallEvent(e);
                    for (int j = 0; j < Event.Listeners.Count; j++) {
                        if (Event.Listeners[j].PluginInfo.Guid == PluginInfos[i].Guid) {
                            Event.Listeners.RemoveAt(j);
                            j--;
                        }
                    }
                    Plugins.Remove(plugin);
                    PluginInfos.RemoveAt(i);
                    return;
                }
            }
        }
        /// <summary>
        /// 加载插件文件夹中所有插件
        /// </summary>
        public static void LoadAllFromPluginDir() {
            DirectoryInfo dir = new DirectoryInfo(PluginPath);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files) {
                Load(file.FullName);
            }
        }
        /// <summary>
        /// 通过启用配置文件自动加载插件
        /// </summary>
        public static void LoadAllFromPlugin() {
            ConfigManager configManager = new ConfigManager(StringUtil.GetSubPath(PluginPath, "Plugins.json"));
            DirectoryInfo dir = new DirectoryInfo(PluginPath);
            foreach (DirectoryInfo PluginDir in dir.GetDirectories()) {
                if (File.Exists(StringUtil.GetSubPath(PluginDir.FullName, "Plugin.dll"))) {
                    bool isEnable = true;
                    try {
                        isEnable = configManager.GetBool(StringUtil.GetSubPath(PluginDir.FullName, "Plugin.dll"));
                    }
                    catch { }
                    if (isEnable) {
                        Load(StringUtil.GetSubPath(PluginDir.FullName, PluginDir.Name + ".dll"));
                    }
                } else if (File.Exists(StringUtil.GetSubPath(PluginDir.FullName, PluginDir.Name + ".dll"))) {
                    bool isEnable = true;
                    try {
                        isEnable = configManager.GetBool(StringUtil.GetSubPath(PluginDir.FullName, PluginDir.Name + ".dll"));
                    }
                    catch { }
                    if (isEnable) {
                        Load(StringUtil.GetSubPath(PluginDir.FullName, PluginDir.Name + ".dll"));
                    }
                }
            }
        }
        /// <summary>
        /// 禁用插件
        /// </summary>
        /// <param name="PluginGuid">插件Guid</param>
        public static void SetDisable(string PluginGuid) {
            ConfigManager configManager = new ConfigManager(StringUtil.GetSubPath(PluginPath, "Plugins.json"));
            configManager.SetBool(PluginGuid, false);
            configManager.SaveConfig();
        }
        /// <summary>
        /// 启用插件
        /// </summary>
        /// <param name="PluginGuid">插件Guid</param>
        public static void SetEnable(string PluginGuid) {
            ConfigManager configManager = new ConfigManager(StringUtil.GetSubPath(PluginPath, "Plugins.json"));
            configManager.SetBool(PluginGuid, false);
            configManager.SaveConfig();
        }
        /// <summary>
        /// 禁用插件
        /// </summary>
        /// <param name="plugin">插件类</param>
        public static void SetDisable(IPlugin plugin) {
            ConfigManager configManager = new ConfigManager(StringUtil.GetSubPath(PluginPath, "Plugins.json"));
            configManager.SetBool(plugin.GetPluginInfo().Guid, false);
            configManager.SaveConfig();
        }
        /// <summary>
        /// 启用插件
        /// </summary>
        /// <param name="plugin">插件类</param>
        public static void SetEnable(IPlugin plugin) {
            ConfigManager configManager = new ConfigManager(StringUtil.GetSubPath(PluginPath, "Plugins.json"));
            configManager.SetBool(plugin.GetPluginInfo().Guid, false);
            configManager.SaveConfig();
        }
        /// <summary>
        /// 卸载所有插件
        /// </summary>
        public static void UnloadAll() {
            foreach (IPlugin plugin in Plugins.ToArray()) {
                UnLoad(plugin);
            }
        }
        /// <summary>
        /// 启用所有已加载插件
        /// </summary>
        public static void EnableAll() {
            foreach (IPlugin plugin in Plugins) {
                plugin.OnEnable();
            }
        }
        /// <summary>
        /// 禁用所有已加载插件
        /// </summary>
        public static void DisableAll() {
            foreach (IPlugin plugin in Plugins) {
                plugin.OnDisable();
            }
        }
    }

}
