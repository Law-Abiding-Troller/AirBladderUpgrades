using HarmonyLib;
using UnityEngine;

namespace AirBladderUpgrades
{
    [HarmonyPatch(typeof(AirBladder))] //patch the air bladder
    public class AirBladderPatches
    {
        static bool collectedDefaultValues = false;
        static float capacityDefaultValue;
        public static bool actually0;

        [HarmonyPatch(nameof(AirBladder.Awake))]
        [HarmonyPrefix]
        public static void Awake_Prefix(AirBladder __instance)
        {
            if (collectedDefaultValues) return;
            capacityDefaultValue = __instance.oxygenCapacity;
            collectedDefaultValues = true;
        }

        [HarmonyPatch(nameof(AirBladder.Update))]
        [HarmonyPostfix]
        public static void Update_Postfix(AirBladder __instance)
        {
            if (__instance == null) return;
            var tempstorage = __instance.GetComponent<StorageContainer>();
            if (Input.GetKeyDown(ModOptions.UpgradesContainerKey)) //check if the keybind to open the storage container is pressed
            {
                Plugin.Logger.LogInfo("Open Storage Container Key pressed for Air Bladder!");
                if (tempstorage.open) //check if its already open
                {
                    Plugin.Logger.LogInfo("The storage container for the Air Bladder is open! Close it to open it!");
                    return;
                }
                tempstorage.Open();//open it
            }
        }

        [HarmonyPatch(nameof(AirBladder.UpdateInflateState))] //patch the updateinflatestate method, which checks the current capacity
        [HarmonyPrefix]
        public static void UpdateInflateState_Prefix(AirBladder __instance) //change the capcity before it checks calls the method
        {
            
            if (__instance == null) //check if the instance is null
            {
                Plugin.Logger.LogWarning("UpdateInflateState_Prefix failed! __instance is null!");
                return;
            }
            if (!actually0)//check if is null or is actually 0 in plugin.
            {
                __instance.oxygenCapacity = capacityDefaultValue;  
            }
            __instance.oxygenCapacity *= Plugin.currentcapacity; //change the o2
            
            Plugin.currentcapacity = 0;//so it doesn't do it again until an item is removed or added
        }

        [HarmonyPatch(nameof(AirBladder.OnDraw))]
        [HarmonyPostfix]
        public static void OnDraw_Postfix(AirBladder __instance)
        {
            if (__instance == null)
            {
                return;
            }
            var tempstorage = __instance.GetComponent<StorageContainer>();
            if (tempstorage == null)
            {
                Plugin.Logger.LogError("tempstorage is null! This should never happen! Something is wrong with Nautilus!");
                return;
            }

            tempstorage.container.onAddItem += Plugin.OnItemAdded;
            tempstorage.container.onRemoveItem += Plugin.OnItemRemoved;
            tempstorage.container._label = "AIR BLADDER";
        }

        [HarmonyPatch(nameof(AirBladder.OnHolster))]
        [HarmonyPostfix]
        public static void OnHolster_Postfix(AirBladder __instance)
        {
            if (__instance == null) return;
            var tempstorage = __instance.GetComponent<StorageContainer>();
            if (tempstorage == null) return;
            tempstorage.container.onAddItem -= Plugin.OnItemAdded;
            tempstorage.container.onRemoveItem -= Plugin.OnItemRemoved;
        }

        [HarmonyPatch(nameof(AirBladder.OnDestroy))]
        [HarmonyPostfix]
        public static void OnDestroy_Postfix(AirBladder __instance)
        {
            __instance.oxygenCapacity = capacityDefaultValue;
        }
    }
}
