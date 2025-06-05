using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using AirBladderUpgrades;

namespace AirBladderUpgrades
{
    [HarmonyPatch(typeof(AirBladder))] //patch the air bladder
    public class AirBladderPatches
    {
        [HarmonyPatch(nameof(AirBladder.UpdateInflateState))] //patch the updateinflatestate method, which checks the current capacity
        [HarmonyPrefix]
        public static void UpdateInflateState_Prefix(AirBladder __instance) //change the capcity before it checks calls the method
        {
            if (__instance == null) //check if the instance is null
            {
                return;
            }
            if (!Plugin.actually0)//check if is null or is actually 0 in plugin.
            {
                __instance.oxygenCapacity = Plugin.capacityDefaultValue;  
            }
            __instance.oxygenCapacity *= Plugin.currentcapacity; //change the o2
            
            Plugin.currentcapacity = 0;//so it doesn't do it again until an item is removed or added
        }
    }
}
