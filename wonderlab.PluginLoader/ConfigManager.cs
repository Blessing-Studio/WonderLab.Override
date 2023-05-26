using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using wonderlab.PluginLoader.Interfaces;

namespace wonderlab.PluginLoader
{
    public class ConfigManager : IDisposable {
        /// <summary>
        /// ���������ֵ�
        /// </summary>
        private static Dictionary<string, Dictionary<string, object>> Config = new Dictionary<string, Dictionary<string, object>>();
        /// <summary>
        /// �ļ�·��
        /// </summary>
        public string FilePath;
        /// <summary>
        /// �����ļ�����
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
        /// ��ȡĳ������ֵ
        /// </summary>
        /// <param name="Key">��</param>
        /// <returns>��Ӧ��ֵ</returns>
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
        /// ���ù�������Ӧ�Ĳ��
        /// </summary>
        public IPlugin? Plugin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin">���ʵ��</param>
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
        /// <param name="ConfigFilePath">�ļ�·��</param>
        public ConfigManager(string ConfigFilePath) {
            FilePath = ConfigFilePath;
            if (!Config.ContainsKey(FilePath)) {
                LoadConfig();
            }
        }
        /// <summary>
        /// ��������
        /// </summary>
        public void SaveConfig() {
            string json = JsonConvert.SerializeObject(Config[FilePath]);
            File.WriteAllText(FilePath, json);
        }
        /// <summary>
        /// ��������
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
        /// ͨ������ȡһ���ַ���
        /// </summary>
        /// <param name="Key">��</param>
        /// <returns>��Ӧ���ַ���</returns>
        public string GetString(string Key) {
            return (string)Config[FilePath][Key];
        }
        /// <summary>
        /// ͨ������ȡһ���ַ�������
        /// </summary>
        /// <param name="Key">��</param>
        /// <returns>��Ӧ���ַ�������</returns>
        public string[] GetStringArray(string Key) {
            return (string[])Config[FilePath][Key];
        }
        /// <summary>
        /// ͨ������ȡһ��32λ����
        /// </summary>
        /// <param name="Key">��</param>
        /// <returns>��Ӧ��32λ����</returns>
        public int GetInt32(string Key) {
            return (int)Config[FilePath][Key];
        }
        /// <summary>
        /// ͨ������ȡһ��32λ��������
        /// </summary>
        /// <param name="Key">��</param>
        /// <returns>��Ӧ��32λ��������</returns>
        public int[] GetInt32Array(string Key) {
            return (int[])Config[FilePath][Key];
        }
        /// <summary>
        /// ��ȡһ��64λ����
        /// </summary>
        /// <param name="Key">��</param>
        /// <returns>��Ӧ��64λ����</returns>
        public long GetLong(string Key) {
            return (long)Config[FilePath][Key];
        }
        /// <summary>
        /// ��ȡһ��64λ��������
        /// </summary>
        /// <param name="Key">��</param>
        /// <returns>��Ӧ��64λ��������</returns>
        public long[] GetLongArray(string Key) {
            return (long[])Config[FilePath][Key];
        }
        /// <summary>
        /// ��ȡһ��object
        /// </summary>
        /// <param name="Key">��</param>
        /// <returns>��Ӧ��object</returns>
        public object GetObject(string Key) {
            return Config[FilePath][Key];
        }
        /// <summary>
        /// ��ȡһ��object����
        /// </summary>
        /// <param name="Key">��</param>
        /// <returns>��Ӧ��object����</returns>
        public object[] GetObjectArray(string Key) {
            return (object[])Config[FilePath][Key];
        }
        /// <summary>
        /// ��ȡһ������ֵ
        /// </summary>
        /// <param name="Key">��</param>
        /// <returns>��Ӧ�Ĳ���ֵ</returns>
        public bool GetBool(string Key) {
            return (bool)Config[FilePath][Key];
        }
        /// <summary>
        /// ��ȡһ������ֵ����
        /// </summary>
        /// <param name="Key">��</param>
        /// <returns>��Ӧ�Ĳ���ֵ����</returns>
        public bool[] GetBoolArray(string Key) {
            return (bool[])Config[FilePath][Key];
        }
        /// <summary>
        /// ��ȡ���������ֵ�
        /// </summary>
        /// <returns>���������ֵ�</returns>
        public Dictionary<string, object> GetConfigDictionary() {
            return Config[FilePath];
        }
        #endregion

        #region Set
        /// <summary>
        /// ���ü���Ӧ���ַ���
        /// </summary>
        /// <param name="Key">��</param>
        /// <param name="Value">�ַ���</param>
        public void SetString(string Key, string Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// ���ü���Ӧ���ַ�������
        /// </summary>
        /// <param name="Key">��</param>
        /// <param name="Value">�ַ�������</param>
        public void SetStringArray(string Key, string[] Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// ���ü���Ӧ��32λ����
        /// </summary>
        /// <param name="Key">��</param>
        /// <param name="Value">32λ����</param>
        public void SetInt32(string Key, int Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// ���ü���Ӧ��32λ��������
        /// </summary>
        /// <param name="Key">��</param>
        /// <param name="Value">32λ��������</param>
        public void SetIntArray(string Key, int[] Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// ���ü���Ӧ��64λ����
        /// </summary>
        /// <param name="Key">��</param>
        /// <param name="Value">64λ����</param>
        public void SetLong(string Key, long Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// ���ü���Ӧ��64λ��������
        /// </summary>
        /// <param name="Key">��</param>
        /// <param name="Value">64λ��������</param>
        public void SetLongArray(string Key, long[] Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// ���ü���Ӧ��object
        /// </summary>
        /// <param name="Key">��</param>
        /// <param name="Value">object</param>
        public void SetObject(string Key, object Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// ���ü���Ӧ��object����
        /// </summary>
        /// <param name="Key">��</param>
        /// <param name="Value">object����</param>
        public void SetObjectArray(string Key, object[] Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// ���ü���Ӧ�Ĳ���ֵ
        /// </summary>
        /// <param name="Key">��</param>
        /// <param name="Value">����ֵ</param>
        public void SetBool(string Key, bool Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// ���ü���Ӧ�Ĳ���ֵ����
        /// </summary>
        /// <param name="Key">��</param>
        /// <param name="Value">����ֵ����</param>
        public void SetBoolArray(string Key, bool[] Value) {
            Config[FilePath][Key] = Value;
        }
        /// <summary>
        /// �������������ֵ�
        /// </summary>
        /// <param name="Value">�ֵ�</param>
        public void SetDictionary(Dictionary<string, object> Value) {
            Config[FilePath] = Value;
        }
        /// <summary>
        /// ��������
        /// </summary>
        public void Dispose() {
            SaveConfig();
        }
        #endregion
        /// <summary>
        /// �Ƴ���
        /// </summary>
        /// <param name="Key">��</param>
        public void Remove(string Key) {
            Config[FilePath].Remove(Key);
        }
    }

}
