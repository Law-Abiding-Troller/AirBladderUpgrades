using HarmonyLib;
using UnityEngine;

namespace AirBladderUpgrades
{
    [HarmonyPatch(typeof(AirBladder))] //patch the air bladder
    public class AirBladderPatches
    {
        static bool _collectedDefaultValues;
        static float _capacityDefaultValue;
        public static bool Actually0;

        [HarmonyPatch(nameof(AirBladder.Awake))]
        [HarmonyPrefix]
        public static void Awake_Prefix(AirBladder __instance)
        {
            if (__instance == null) return; 
            var tempstorage = __instance.GetComponent<StorageContainer>();
            if (tempstorage == null) return;
            tempstorage.container.onAddItem += Plugin.OnItemAdded;
            tempstorage.container.onRemoveItem += Plugin.OnItemRemoved;
            tempstorage.container._label = "AIR BLADDER";
            if (_collectedDefaultValues) return;
            _capacityDefaultValue = __instance.oxygenCapacity;
            _collectedDefaultValues = true;
        }

        [HarmonyPatch(nameof(AirBladder.Update))]
        [HarmonyPostfix]
        public static void Update_Postfix(AirBladder __instance)
        {
            if (__instance == null) return;
            var tempstorage = __instance.GetComponent<StorageContainer>();
            if (tempstorage == null) return;
            if (Input.GetKeyDown(ModOptions.UpgradesContainerKey)) //check if the keybind to open the storage container is pressed
            {
                Plugin.Logger.LogInfo("Open Storage Container Key pressed for Air Bladder!");
                if (tempstorage.open) //check if its already open
                {
                    Plugin.Logger.LogInfo("The storage container for the Air Bladder is open! Close it to open it!");
                    ErrorMessage.AddMessage("Close 'AIR BLADDER' to open it!"); 
                    return;
                }
                tempstorage.Open();//open it
            }
        }

        [HarmonyPatch(nameof(AirBladder.UpdateInflateState))] //patch the updateinflatestate method, which checks the current capacity
        [HarmonyPrefix]
        public static void UpdateInflateState_Prefix(AirBladder __instance) //change the capcity before it checks calls the method
        {

            if (__instance == null) return; //check if the instance is null
            if (!Actually0)//check if is null or is actually 0 in plugin.
            {
                __instance.oxygenCapacity = _capacityDefaultValue;  
            }
            __instance.oxygenCapacity *= Plugin.currentcapacity; //change the o2
            
            Plugin.currentcapacity = 0;//so it doesn't do it again until an item is removed or added
        }

        [HarmonyPatch(nameof(AirBladder.OnDraw))]
        [HarmonyPostfix]
        public static void OnDraw_Postfix(AirBladder __instance)//kept just for good measure
        {
            if (__instance == null)
            {
                return;
            }
            
        }

        [HarmonyPatch(nameof(AirBladder.OnHolster))]
        [HarmonyPostfix]
        public static void OnHolster_Postfix(AirBladder __instance)//kept just for good measure
        {
            if (__instance == null) return;
            
        }

        [HarmonyPatch(nameof(AirBladder.OnDestroy))]
        [HarmonyPostfix]
        public static void OnDestroy_Postfix(AirBladder __instance)
        {
            __instance.oxygenCapacity = _capacityDefaultValue;
            var tempstorage = __instance.GetComponent<StorageContainer>();
            if (tempstorage == null) return;
            tempstorage.container.onAddItem -= Plugin.OnItemAdded;
            tempstorage.container.onRemoveItem -= Plugin.OnItemRemoved;
        }
    }
}
