using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using AirBladderUpgrades;

namespace AirBladderUpgrades
{
    [HarmonyPatch(typeof(AirBladder))]
    public class AirBladderPatches
    {
        [HarmonyPatch(nameof(AirBladder.UpdateInflateState))]
        [HarmonyPrefix]
        public static void UpdateInflateState_Prefix(AirBladder __instance)
        {
            if (__instance == null)
            {
                return;
            }
            Plugin.defaults[Plugin.select] = __instance.oxygenCapacity;
            __instance.oxygenCapacity += Plugin.currentcapacity;
            Plugin.currentcapacity = 0;
        }
    }
}
