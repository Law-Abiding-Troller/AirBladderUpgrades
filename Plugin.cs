﻿using System.Collections;
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
    [BepInPlugin("com.lawabidingtroller.airbladderupgrades", "AirBladderUpgrades", "0.0.2")]//next version to release
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public IEnumerator SetAirBladderUpgrades(TechType tech) //actually create the container
        {
            Logger.LogInfo("Creating Air Bladder Storage...");//log EVERYTHING
            Logger.LogInfo($"Fetching Air Bladder Prefab...");
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(tech);//fetch the prefab
            yield return task;//wait for prefab to finish
            Logger.LogInfo("Prefab Fetched Successfully!");
            GameObject prefab = task.GetResult();//get the prefab
            Logger.LogInfo($"The prefab for the Air Bladder is {prefab}");//log it because why not
            PrefabUtils.AddStorageContainer(prefab, "AirBladderStorage", "AirBladderStorageChild", 2, 2, true);//use nautilus's method to create the storage container
            Logger.LogInfo("Storage Container Added. If it opens, the task was successful");//log it
        }

        public static ModOptions ModOptions;//should change to be a nautilus class or keep as is (currently my class in the Config.cs file)?
        
        public static TechGroup toolupgrademodules = EnumHandler.AddEntry<TechGroup>("myGroup")
                .WithPdaInfo("Tools Upgrade Modules");
        public static TechCategory AirBladderUpgrades  = EnumHandler.AddEntry<TechCategory>("myCategory")
            .WithPdaInfo("Air Bladder Upgrades")
            .RegisterToTechGroup(toolupgrademodules);
        
        private void Awake()
        {
            // set project-scoped logger instance
            Logger = base.Logger;

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"AirBladderUpgrades");
            Logger.LogInfo($"Awake method is running. Dependencies exist. Completing plugin load...");
            Logger.LogWarning("WARNING! THIS IS A TEST BUILD! EXPECT MANY BUGS!");

            StartCoroutine(SetAirBladderUpgrades(TechType.AirBladder));//call the method
            InitializePrefabs();//initialize the custom upgrades for this mod

            Logger.LogInfo("Creating Mod Options...");
            ModOptions = OptionsPanelHandler.RegisterModOptions<ModOptions>();//register the mod options

            Logger.LogInfo("Adding the Air Bladder tab to the fabricator...");//create a tab for the air bladder and its upgrades
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "AirBladderTab", "Air Bladder", SpriteManager.Get(TechType.AirBladder), "Personal", "Tools");
            Logger.LogInfo("Adding the Air Bladder to the Air Bladder tab in the fabricator under Personal -> Tools..."); //move the air bladder to said tab
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.AirBladder, "Personal", "Tools", "AirBladderTab");
            Logger.LogInfo("Removing the Air Bladder from the Tools tab in the Fabricator under Personal -> Tools"); //remove the air bladder from the tools tab
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, "Personal", "Tools", "Air bladder");
            
            
            
            
            Logger.LogInfo("Plugin fully loaded successfully!"); //thats all for this method, so fully loaded is sufficeint
        }
        int timer = 0; //to not have the log filled with too many logs
        public static float currentfloatspeed; //is the current air bladder float speed from the upgrade
        public static float currentcapacity; //current amount to increase the air bladder capacity in the patch (in Patches.cs) based on the upgrade
        public bool upgraded = false; //subscribe to the delegate only once
        public bool subscribed = false; //subscribe to the delecate only once
        public static bool collectedDefaultValues;
        public static float capacityDefaultValue;
        public void Update()
        {
            timer++; //tick up the timer, so that it isnt just stuck
            if (timer == 5000) //reset the timer after enough time
            {
                Logger.LogInfo("Timer Reset.");
                timer = 0;
            }
            if (Inventory.main == null) //check if inventory.main is null, so that it doesn't try anything and break
            {
                if (timer == 0)
                {
                    Logger.LogInfo("Inventory.main is Null! Likely because a save hasn't loaded yet");
                }
                return;
            }
            PlayerTool heldtool = Inventory.main.GetHeldTool(); //actually get the tool the player is holding
            if (heldtool == null) //return if the player isnt holding a tool
            {
                if (timer == 0)
                {
                    Logger.LogInfo("heldtool is Null! Likely because the player isn't holding a tool!");
                }
                return;
            }
            if (heldtool is AirBladder) //chech if the tool is the air bladder
            {
                var instance = heldtool as AirBladder; //get the air bladder instance
                if (instance == null) //check if somehow null
                {
                    if (timer == 0)
                    {
                        Logger.LogInfo("instance is Null! Further debugging required!");
                    }
                    return;
                }

                if (!collectedDefaultValues)
                {
                    capacityDefaultValue = instance.oxygenCapacity;//get the default value for when the capacity is 0
                    
                    collectedDefaultValues = true;
                }
                var tempstorage = heldtool.gameObject.GetComponent<StorageContainer>(); //get the storage container component for that specific air bladder
                if (tempstorage == null) //chech if somehow null which would mean something in the SetAirBladderUpgrades method broke
                {
                    if (timer == 0)
                    {
                        Logger.LogInfo("tempstorage is Null! Likely because something failed in the SetAirBladderUpgrades method!");
                    }
                    return;
                }
                if (!subscribed) //subscribe to the onAddItem delegate
                {
                    tempstorage.container.onAddItem += OnItemAdded;
                    tempstorage.container.onRemoveItem += OnItemRemoved;
                    subscribed = true;
                }
                //set the allowed tech for the air bladder storage container
                var allowedtech = new TechType[] { TechType.Bleach, AirBladderCapacityUpgradeMk1.mk1capacityprefabinfo.TechType, AirBladderCapacityUpgradeMk2.mk2capacityprefabinfo.TechType, AirBladderCapacityUpgradeMk3.mk3capacityprefabinfo.TechType };
                tempstorage.container.SetAllowedTechTypes(allowedtech); //actually use the array to set allowed tech
                if (Input.GetKeyDown(ModOptions.OpenUpgradesContainerKey)) //check if the keybind to open the storage container is pressed
                {
                    Logger.LogInfo("Open Storage Container Key pressed for Air Bladder!");
                    if (tempstorage.open) //check if its already open
                    {
                        if (timer == 0)
                        {
                            Logger.LogInfo("The storage container for the Air Bladder is open! Close it to open it!");
                        }
                        return;
                    }
                    tempstorage.container._label = "AIR BLADDER"; //set the proper lable
                    tempstorage.Open(); //actually open it
                }
                
            }
        }

        private void OnItemRemoved(InventoryItem item) //custom behavior for when one of the upgrades (or bleach) is removed from the storage container
        {
            if (item == null || item.item == null) //check if its somehow null
            {
                return;
            }
            if (item.item.GetTechType() == TechType.Bleach) //give the ability to use the air bladder back to the player
            {
                Logger.LogInfo("Bleach has been removed from the storage container. The player is safe.");
                currentcapacity = 5f;
            }
            //reduce the capacity when the mk1 upgrade is removed, check if it has been already upgraded, let me know if it should cause issues on reload
            if (item.item.GetTechType() == AirBladderCapacityUpgradeMk1.mk1capacityprefabinfo.TechType && upgraded)
            {
                currentcapacity = 0 - 18f;
                upgraded = false;
            }
            //same as mk1, but more
            if (item.item.GetTechType() == AirBladderCapacityUpgradeMk2.mk2capacityprefabinfo.TechType && upgraded)
            {
                currentcapacity =  0 - 48f;
                upgraded = false;
            }
            //same as mk2, but even more
            if (item.item.GetTechType() == AirBladderCapacityUpgradeMk3.mk3capacityprefabinfo.TechType && upgraded)
            {
                currentcapacity = 0 - 78f;
                upgraded = false;
            }
        }

        private void OnItemAdded(InventoryItem item)//custom behavior for if any of the upgrades (or bleach) has been added to the storage container
        {
            if (item == null || item.item == null) //check if its somehow null
            {
                return;
            }
            //remove the oxygen for the player to use the air bladder and kill themselfs with bleach in the air bladder, practically making it permanently empty. to make it more realistic
            if (item.item.GetTechType() == TechType.Bleach)
            {
                Logger.LogInfo("Bleach has been added to the storage container! The player is going to die if they inhale it!");
                currentcapacity = 0;
            }
            //increase for the mk1 to go into the patches
            if (item.item.GetTechType() == AirBladderCapacityUpgradeMk1.mk1capacityprefabinfo.TechType && !upgraded)
            {
                currentcapacity = 18f;
                upgraded = true;
            }
            //same as mk1, but more
            if (item.item.GetTechType() == AirBladderCapacityUpgradeMk2.mk2capacityprefabinfo.TechType && !upgraded)
            {
                currentcapacity = 48f;
                upgraded = true;
            }
            //ame as mk 2, but even more
            if (item.item.GetTechType() == AirBladderCapacityUpgradeMk3.mk3capacityprefabinfo.TechType && !upgraded)
            {
                currentcapacity = 78f;
                upgraded = true;
            }
        }

        private void InitializePrefabs() //actually register all of the custom prefabs
        {
            Logger.LogInfo("Initializing prefabs...");
            AirBladderCapacityUpgradeMk1.Register();
            AirBladderCapacityUpgradeMk2.Register();
            AirBladderCapacityUpgradeMk3.Register();
            Logger.LogInfo("Most, if not, all, prefabs initalized successfully!");
        }
    }
}