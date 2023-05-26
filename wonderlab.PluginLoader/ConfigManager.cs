using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using wonderlab.PluginLoader.Interfaces;

namespace wonderlab.PluginLoader
{
    public class ConfigManager : IDisposable {
        /// <summary>
        /// 配置数据字典
        /// </summary>
        private static Dictionary<string, Dictionary<string, object>> Config = new Dictionary<string, Dictionary<string, object>>();
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath;
        /// <summary>
        /// 配置文件数据
        /// </summary>
        public string? ConfigData
        {
            get
            {
                return File.ReadAllText(FilePath);
            }
            set
            {
                File.WriteAllText(FilePath, value);
            }
        }
        /// <summary>
        /// 获取某个键的值
        /// </summary>
        /// <param name="Key">键</param>
        /// <returns>对应的值</returns>
        public object this[string Key]
        {
            get
            {
                return Config[FilePath][Key];
            }
            set
            {
                Config[FilePath][Key] = value;
            }
        }
        /// <summary>
        /// 配置管理器对应的插件
        /// </summary>
        public IPlugin? Plugin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin">插件实例</param>
        public ConfigManager(IPlugin plugin) {
            Plugin = plugin;
            string tmp = StringUtil.GetSubPath(PluginLoader.PluginPath, Plugin.GetPluginInfo().Name);
            FilePath = StringUtil.GetSubPath(tmp, "Config.json");
            if (!Config.ContainsKey(Plugin.GetPluginInfo().Guid)) {
                LoadConfig();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConfigFilePath">文件路径</param>
        public ConfigManager(string ConfigFilePath) {
            FilePath = ConfigFilePath;
            if (!Config.ContainsKey(FilePath)) {
                LoadConfig();
            }
        }
        /// <summary>
        /// 保存配置
        /// </summary>
        public void SaveConfig() {
            string json = JsonConvert.SerializeObject(Config[FilePath]);
            File.WriteAllText(FilePath, json);
        }
        /// <summary>
        /// 加载配置
        /// </summary>
        public void LoadConfig() {
            if (!File.Exists(FilePath)) {
                new FileInfo(FilePath).Directory!.Create();
                File.Create(FilePath).Close();
            }
            Dictionary<string, object>? tmp = JsonConvert.DeserializeObject<Dictionary<string, object>>(ConfigData!);

            if (tmp == null) {
                Config[FilePath] = new Dictionary<string, object>();
            } else { Config[FilePath] = tmp; }
        }
        #region Get
        /// <summary>
        /// 通过键获取一个字符串
        /// </summary>
        /// <param name="Key">键</param>
        /// <returns>对应的字符串</returns>
        public string GetString(string Key) {
            return (string)Config[FilePath][Key];
        }
        /// <summary>
        /// 通过键获取一个字符串数组
        /// </summary>
        /// <param name="Key">键</param>
        /// <returns>对应的字符串数组</returns>
        public string[] GetStringArray(string Key) {
            return (string[])Config[FilePath][Key];
        }
        /// <summary>
        /// 通过键获取一个32位整数
        /// </summary>
        /// <param name="Key">键</param>
        /// <returns>对应的32位整数</returns>
        public int GetInt32(string Key) {
            return (int)Config[FilePath][Key];
        }
        /// <summary>
        /// 通过键获取一个32位整数数组
        /// </summary>
        /// <param name="Key">键</param>
        /// <returns>对应的32位整数数组</returns>
        public int[] GetInt32Array(string Key) {
            return (int[])Config[FilePath][Key];
        }
        /// <summary>
        /// 获取一个64位整数
        /// </summary>
        /// <param name="Key">键</param>
        /// <returns>对应的64位整数</returns>
        public long GetLong(string Key) {
            return (long)Config[FilePath][Key];
        }
        /// <summary>
        /// 获取一个64位整数数组
        /// </summary>
        /// <param name="Key">键</param>
        /// <returns>对应的64位整数数组</returns>
        public long[] GetLongArray(string Key) {
            return (long[])Config[FilePath][Key];
        }
        /// <summary>
        /// 获取一个object
        /// </summary>
        /// <param name="Key">键</param>
        /// <returns>对应的object</returns>
        public object GetObject(string Key) {
            return Config[FilePath][Key];
        }
        /// <summary>
        /// 获取一个object数组
        /// </summary>
        /// <param name="Key">键</param>
        /// <returns>对应的object数组</returns>
        public object[] GetObjectArray(string Key) {
            return (object[])Config[FilePath][Key];
        }
        /// <summary>
        /// 获取一个布尔值
        /// </summary>
        /// <param name="Key">键</param>
        /// <returns>对应的布尔值</returns>
        public bool GetBool(string Key) {
            return (bool)Config[FilePath][Key];
        }
        /// <summary>
        /// 获取一个布儿值数组
        /// </summary>
        /// <param name="Key">键</param>
        /// <returns>对应的布尔值数组</returns>
        public bool[] GetBoolArray(string Key) {
            return (bool[])Config[FilePath][Key];
        }
        /// <summary>
        /// 获取配置数据字典
        /// </summary>
        /// <returns>配置数据字典</returns>
        public Dictionary<string, object> GetConfigDictionary() {
            return Config[FilePath];
        }
        #endregion

        #region Set
        /// <summary>
        /// 设置键对应的字符串
        /// </summary>
        /// <param name="Key">键</param>
        /// <param name="Value">字符串</param>
        public void SetString(string Key, string Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// 设置键对应的字符串数组
        /// </summary>
        /// <param name="Key">键</param>
        /// <param name="Value">字符串数组</param>
        public void SetStringArray(string Key, string[] Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// 设置键对应的32位整数
        /// </summary>
        /// <param name="Key">键</param>
        /// <param name="Value">32位整数</param>
        public void SetInt32(string Key, int Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// 设置键对应的32位整数数组
        /// </summary>
        /// <param name="Key">键</param>
        /// <param name="Value">32位整数数组</param>
        public void SetIntArray(string Key, int[] Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// 设置键对应的64位整数
        /// </summary>
        /// <param name="Key">键</param>
        /// <param name="Value">64位整数</param>
        public void SetLong(string Key, long Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// 设置键对应的64位整数数组
        /// </summary>
        /// <param name="Key">键</param>
        /// <param name="Value">64位整数数组</param>
        public void SetLongArray(string Key, long[] Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// 设置键对应的object
        /// </summary>
        /// <param name="Key">键</param>
        /// <param name="Value">object</param>
        public void SetObject(string Key, object Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// 设置键对应的object数组
        /// </summary>
        /// <param name="Key">键</param>
        /// <param name="Value">object数组</param>
        public void SetObjectArray(string Key, object[] Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// 设置键对应的布尔值
        /// </summary>
        /// <param name="Key">键</param>
        /// <param name="Value">布尔值</param>
        public void SetBool(string Key, bool Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// 设置键对应的布尔值数组
        /// </summary>
        /// <param name="Key">键</param>
        /// <param name="Value">布尔值数组</param>
        public void SetBoolArray(string Key, bool[] Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// 设置配置数据字典
        /// </summary>
        /// <param name="Value">字典</param>
        public void SetDictionary(Dictionary<string, object> Value) {
            Config[FilePath] = Value;
        }
        /// <summary>
        /// 保存配置
        /// </summary>
        public void Dispose() {
            SaveConfig();
        }
        #endregion
        /// <summary>
        /// 移除键
        /// </summary>
        /// <param name="Key">键</param>
        public void Remove(string Key) {
            Config[FilePath].Remove(Key);
        }
    }

}
