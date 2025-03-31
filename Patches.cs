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
            __instance.oxygenCapacity += Plugin.currentcapacity; //add, not going to be that much
            Plugin.currentcapacity = 0;
        }
    }
}
