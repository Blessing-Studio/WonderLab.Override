using System.Collections;
using System.Diagnostics.CodeAnalysis;
using wonderlab.PluginLoader.Interfaces;

namespace wonderlab.PluginLoader
{
    /// <summary>
    /// ��ȫ��ȡһ������
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    public class Config<T> {
        /// <summary>
        /// ���ù�����
        /// </summary>
        public readonly ConfigManager ConfigManager;
        /// <summary>
        /// ��
        /// </summary>
        public readonly string Key;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configManager">���ù�����</param>
        /// <param name="key">��</param>
        public Config(ConfigManager configManager, string key) {
            ConfigManager = configManager;
            Key = key;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin">���ʵ��</param>
        /// <param name="key">��</param>
        public Config(IPlugin plugin, string key) {
            ConfigManager = new ConfigManager(plugin);
            Key = key;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">�ļ�·��</param>
        /// <param name="key">��</param>
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
        /// ��������
        /// </summary>
        public void SaveConfig() {
            ConfigManager.SaveConfig();
        }
        /// <summary>
        /// �Ƴ��˼�
        /// </summary>
        public void Remove() {
            ConfigManager.Remove(Key);
        }
        /// <summary>
        /// ��ȡ�����õ�ֵ
        /// </summary>
        /// <returns>�����õ�ֵ</returns>
        public T? Get() {
            try {
                return (T)ConfigManager.GetObject(Key);
            }
            catch {
                return default;
            }
        }
        /// <summary>
        /// ���ô����õ�ֵ
        /// </summary>
        /// <param name="Value">Ҫ���õ�ֵ</param>
        public void Set(T? Value) {
            if (Value == null) {
                return;
            }
            ConfigManager.SetObject(Key, Value);
        }
        /// <summary>
        /// ��ȡ���ù�����
        /// </summary>
        /// <returns>���ù�����</returns>
        public ConfigManager GetConfigManager() {
            return ConfigManager;
        }
    }
}
