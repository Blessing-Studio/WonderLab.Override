using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using wonderlab.PluginLoader.Interfaces;

namespace wonderlab.PluginLoader
{
    public class ConfigManager : IDisposable
    {
        private static Dictionary<string,Dictionary<string, object>> Config = new Dictionary<string, Dictionary<string, object>>();

        public string? configData, FilePath;

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
        public IPlugin? Plugin { get; set; }

        public ConfigManager(IPlugin plugin)
        {
            Plugin = plugin;
            string tmp = StringUtil.GetSubPath(PluginLoader.PluginPath, Plugin.GetPluginInfo().Name);
            FilePath = StringUtil.GetSubPath(tmp, "Config.json");
            if (!Config.ContainsKey(Plugin.GetPluginInfo().Guid))
            {
                LoadConfig();
            }
        }

        public ConfigManager(string ConfigFilePath)
        {
            FilePath = ConfigFilePath;
            if (!Config.ContainsKey(FilePath))
            {
                LoadConfig();
            }
        }

        public void SaveConfig()
        {
            string json = JsonConvert.SerializeObject(Config[FilePath]);
            File.WriteAllText(FilePath, json);
        }

        public void LoadConfig()
        {
            if (!File.Exists(FilePath))
            {
                new FileInfo(FilePath).Directory.Create();
                File.Create(FilePath).Close();
            }
            configData = File.ReadAllText(FilePath);
            Dictionary<string,object>? tmp = JsonConvert.DeserializeObject<Dictionary<string, object>>(configData);

            if (tmp == null)
            {
                Config[FilePath] = new Dictionary<string, object>();
            }
            else { Config[FilePath] = tmp;}
        }
        #region Get
        public string GetString(string Key)
        {
            return (string)Config[FilePath][Key];
        }

        public string[] GetStringArray(string Key)
        {
            return (string[])Config[FilePath][Key];
        }

        public int GetInt32(string Key)
        {
            return (int)Config[FilePath][Key];
        }

        public int[] GetInt32Array(string Key)
        {
            return (int[])Config[FilePath][Key];
        }

        public long GetLong(string Key)
        {
            return (long)Config[FilePath][Key];
        }

        public long[] GetLongArray(string Key)
        {
            return (long[])Config[FilePath][Key];
        }

        public object GetObject(string Key)
        {
            return Config[FilePath][Key];
        }

        public object[] GetObjectArray(string Key)
        {
            return (object[])Config[FilePath][Key];
        }

        public bool GetBool(string Key)
        {
            return (bool)Config[FilePath][Key];
        }

        public bool[] GetBoolArray(string Key)
        {
            return (bool[])Config[FilePath][Key];
        }

        public Dictionary<string,object> GetConfigDictionary()
        {
            return Config[FilePath];
        }
        #endregion

        #region Set
        public void SetString(string Key,string Value)
        {
            Config[FilePath][Key] = Value;
        }

        public void SetStringArray(string Key, string[] Value)
        {
            Config[FilePath][Key] = Value;
        }

        public void SetInt32(string Key, int Value)
        {
            Config[FilePath][Key] = Value;
        }

        public void SetIntArray(string Key, int[] Value)
        {
            Config[FilePath][Key] = Value;
        }

        public void SetLong(string Key, long Value)
        {
            Config[FilePath][Key] = Value;
        }

        public void SetLongArray(string Key, long[] Value)
        {
            Config[FilePath][Key] = Value;
        }

        public void SetObject(string Key, object Value)
        {
            Config[FilePath][Key] = Value;
        }

        public void SetObjectArray(string Key, object[] Value)
        {
            Config[FilePath][Key] = Value;
        }

        public void SetBool(string Key,bool Value)
        {
            Config[FilePath][Key] = Value;
        }

        public void SetBoolArray(string Key, bool[] Value)
        {
            Config[FilePath][Key] = Value;
        }

        public void SetDictionary(Dictionary<string,object> Value)
        {
            Config[FilePath] = Value;
        }

        public void Dispose()
        {
            SaveConfig();
        }
        #endregion


    }
}
