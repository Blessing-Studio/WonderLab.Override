using HarmonyLib;
using System.Numerics;
using wonderlab.ViewModels.Windows;

namespace PatchTest
{
    [HarmonyPatch(typeof(MainWindowViewModel))]
    [HarmonyPatch("get_NotificationCountText")]
    class Patch
    {
        public static void Postfix(ref string __result)
        {
            __result = "hi";
        }
    }
}