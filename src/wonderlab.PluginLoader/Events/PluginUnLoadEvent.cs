
namespace wonderlab.PluginLoader.Events
{
    public class PluginUnLoadEvent : Event
    {
        public override string Name { get { return "PluginUnLoadEvent"; } }
        public PluginInfo PluginInfo { get; set; }
        public override bool Do()
        {
            return true;
        }
    }
}
