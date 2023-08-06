using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol.Server
{
    public class Status
    {
        public string JsonData;
        public string Version;
        public int Protocol;
        public int MaxPlayer;
        public int OnlinePlayer;
        public Chat Description;
        public string Icon { get
            {
                return JObject.Parse(JsonData)["favicon"]!.ToString();
            }
        }
        public Status(string jsonData) 
        {
            JsonData = jsonData;
            JObject obj = JObject.Parse(jsonData);
            Version = obj["version"]!["name"]!.ToString();
            Protocol = (int)obj["version"]!["protocol"]!;
            MaxPlayer = (int)obj["players"]!["max"]!;
            OnlinePlayer = (int)obj["players"]!["online"]!;
            Description = Chat.FromJson(obj["description"]!.ToString());
        }
        public Status()
        {
            Version = "1.18.2";
            Protocol = 761;
            Description = new(string.Empty);
            JObject obj = new();
            obj["version"]!["name"] = Version;
            obj["version"]!["protocol"] = Protocol;
            obj["players"]!["max"] = MaxPlayer;
            obj["players"]!["online"] = OnlinePlayer;
            obj["description"] = Description.JsonData;
            JsonData = obj.ToString();
        }
    }
}
