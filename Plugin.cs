using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Utility;
using UnityEngine;
using AirBladderUpgrades;
using System;
using AirBladderUpgrades.Items.Capacity_Upgrades;
using Nautilus.Handlers;
using JetBrains.Annotations;
using Nautilus.Options;

namespace AirBladderUpgrades
{
    [BepInPlugin("com.lawabidingtroller.airbladderupgrades", "AirBladderUpgrades", "0.0.2")]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public IEnumerator SetAirBladderUpgrades(TechType tech)
        {
            Logger.LogInfo("Creating Air Bladder Storage...");
            Logger.LogInfo($"Fetching Air Bladder Prefab...");
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(tech);
            yield return task;
            Logger.LogInfo("Prefab Fetched Successfully!");
            GameObject prefab = task.GetResult();
            Logger.LogInfo($"The prefab for the Air Bladder is {prefab}");
            PrefabUtils.AddStorageContainer(prefab, "AirBladderStorage", "AirBladderStorageChild", 2, 2, true);
            Logger.LogInfo("Storage Container Added. If it opens, the task was successful");
        }

        public static ModOptions ModOptions;

        private void Awake()
        {
            // set project-scoped logger instance
            Logger = base.Logger;

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"AirBladderUpgrades");
            Logger.LogInfo($"Awake method is running. Dependencies exist. Completing plugin load...");
            Logger.LogWarning("WARNING! THIS IS A TEST BUILD! EXPECT MANY BUGS!");

            SetAirBladderUpgrades(TechType.AirBladder);
            InitializePrefabs();

            Logger.LogInfo("Creating Mod Options...");
            ModOptions = OptionsPanelHandler.RegisterModOptions<ModOptions>();

            Logger.LogInfo("Adding the Air Bladder tab to the fabricator...");
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "AirBladderTab", "Air Bladder", SpriteManager.Get(TechType.AirBladder), "Personal", "Tools");
            Logger.LogInfo("Adding the Air Bladder to the Air Bladder tab in the fabricator under Personal -> Tools...");
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.AirBladder, "Personal", "Tools", "AirBladderTab");
            Logger.LogInfo("Removing the Air Bladder from the Tools tab in the Fabricator under Personal -> Tools");
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, "Personal", "Tools", "Air bladder");

            Logger.LogInfo("Plugin fully loaded successfully!");
        }
        int timer = 0;
        public static float currentfloatspeed;
        public static float currentcapacity;
        public static float[] defaults;
        public static int select = 0;
        public bool upgraded = false;
        public bool subscribed = false;
        public void Update()
        {
            timer++;
            if (Inventory.main == null)
            {
                return;
            }
            PlayerTool heldtool = Inventory.main.GetHeldTool();
            if (heldtool == null)
            {
                return;
            }
            if (heldtool is AirBladder)
            {
                var instance = heldtool as AirBladder;
                if (instance == null)
                {
                    return;
                }
                var tempstorage = heldtool.gameObject.GetComponent<StorageContainer>();
                if (tempstorage == null)
                {
                    return;
                }
                if (!subscribed)
                {
                    tempstorage.container.onAddItem += OnItemAdded;
                    tempstorage.container.onRemoveItem += OnItemRemoved;
                    subscribed = true;
                }
                var allowedtech = new TechType[] { TechType.Bleach, AirBladderCapacityUpgradeMk1.mk1capacityprefabinfo.TechType };
                if (allowedtech == null)
                {
                    return;
                }
                tempstorage.container.SetAllowedTechTypes(allowedtech);
                if (Input.GetKeyDown(ModOptions.OpenUpgradesContainerKey))
                {
                    Logger.LogInfo("Open Storage Container Key pressed for Air Bladder!");
                    if (tempstorage.open)
                    {
                        return;
                    }
                    tempstorage.container._label = "AIR BLADDER";
                    tempstorage.Open();
                }
                
            }
        }

        private void OnItemRemoved(InventoryItem item)
        {
            if (item == null || item.item == null)
            {
                return;
            }
            if (item.item.GetTechType() == TechType.Bleach)
            {
                Logger.LogInfo("Bleach has been removed from the storage container. The player is safe.");
                currentcapacity = 12f;
            }
            if (item.item.GetTechType() == AirBladderCapacityUpgradeMk1.mk1capacityprefabinfo.TechType && upgraded)
            {
                currentcapacity = 0 - 18f;
                upgraded = false;
            }
            if (item.item.GetTechType() == AirBladderCapacityUpgradeMk2.mk2capacityprefabinfo.TechType && upgraded)
            {
                currentcapacity =  0 - 48f;
                upgraded = false;
            }
            if (item.item.GetTechType() == AirBladderCapacityUpgradeMk3.mk3capacityprefabinfo.TechType && upgraded)
            {
                currentcapacity = 0 - 78f;
                upgraded = false;
            }
        }

        private void OnItemAdded(InventoryItem item)
        {
            if (item == null || item.item == null)
            {
                return;
            }
            if (item.item.GetTechType() == TechType.Bleach)
            {
                Logger.LogInfo("Bleach has been added to the storage container! The player is going to die if they inhale it!");
                currentcapacity = 0 - 12f;
            }
            if (item.item.GetTechType() == AirBladderCapacityUpgradeMk1.mk1capacityprefabinfo.TechType && !upgraded)
            {
                currentcapacity = 18f;
                upgraded = true;
            }
            if (item.item.GetTechType() == AirBladderCapacityUpgradeMk2.mk2capacityprefabinfo.TechType && !upgraded)
            {
                currentcapacity = 48f;
                upgraded = true;
            }
            if (item.item.GetTechType() == AirBladderCapacityUpgradeMk3.mk3capacityprefabinfo.TechType && !upgraded)
            {
                currentcapacity = 78f;
                upgraded = true;
            }
        }

        private void InitializePrefabs()
        {
            Logger.LogInfo("Initializing prefabs...");
            AirBladderCapacityUpgradeMk1.Register();
            AirBladderCapacityUpgradeMk2.Register();
            AirBladderCapacityUpgradeMk3.Register();
            Logger.LogInfo("Most, if not, all, prefabs initalized successfully!");
        }
    }
}
