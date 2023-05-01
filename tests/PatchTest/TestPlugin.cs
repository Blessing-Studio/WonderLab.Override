using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;
using wonderlab;
using wonderlab.PluginLoader.Handlers;
using wonderlab.PluginLoader.Interfaces;

namespace PatchTest
{
    [PluginHandler("Test", "注入测试", "1.0.0", "{F5EA993F-2C22-4E18-967F-FECB5BEB9EB7}", "Ddggdd135")]
    internal class TestPlugin : IPlugin
    {
        void IPlugin.OnDisable()
        {
            
        }

        bool IPlugin.OnEnable()
        {
            HarmonyLib.Harmony harmony = new("test");
            harmony.PatchAll();
            return true;
        }

        void IPlugin.OnLoad()
        {
            
        }

        void IPlugin.OnUnload()
        {
            
        }
    }
}
