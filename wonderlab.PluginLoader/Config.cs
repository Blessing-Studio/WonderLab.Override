using System.Collections;
using System.Diagnostics.CodeAnalysis;
using wonderlab.PluginLoader.Interfaces;

namespace wonderlab.PluginLoader
{
    /// <summary>
    /// 安全存取一个配置
    /// </summary>
    /// <typeparam name="T">配置类型</typeparam>
    public class Config<T> {
        /// <summary>
        /// 配置管理器
        /// </summary>
        public readonly ConfigManager ConfigManager;
        /// <summary>
        /// 键
        /// </summary>
        public readonly string Key;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configManager">配置管理器</param>
        /// <param name="key">键</param>
        public Config(ConfigManager configManager, string key) {
            ConfigManager = configManager;
            Key = key;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin">插件实例</param>
        /// <param name="key">键</param>
        public Config(IPlugin plugin, string key) {
            ConfigManager = new ConfigManager(plugin);
            Key = key;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="key">键</param>
        public Config(string path, string key) {
            ConfigManager = new ConfigManager(path);
            Key = key;
        }
        /// <summary>
        /// 
        /// </summary>
        ~Config() {
            SaveConfig();
        }
        /// <summary>
        /// 保存配置
        /// </summary>
        public void SaveConfig() {
            ConfigManager.SaveConfig();
        }
        /// <summary>
        /// 移除此键
        /// </summary>
        public void Remove() {
            ConfigManager.Remove(Key);
        }
        /// <summary>
        /// 读取此配置的值
        /// </summary>
        /// <returns>此配置的值</returns>
        public T? Get() {
            try {
                return (T)ConfigManager.GetObject(Key);
            }
            catch {
                return default;
            }
        }
        /// <summary>
        /// 设置此配置的值
        /// </summary>
        /// <param name="Value">要设置的值</param>
        public void Set(T? Value) {
            if (Value == null) {
                return;
            }
            ConfigManager.SetObject(Key, Value);
        }
        /// <summary>
        /// 获取配置管理器
        /// </summary>
        /// <returns>配置管理器</returns>
        public ConfigManager GetConfigManager() {
            return ConfigManager;
        }
    }
}
