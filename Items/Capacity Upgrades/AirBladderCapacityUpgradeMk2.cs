using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirBladderUpgrades;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;
using UpgradesLIB.Items.Equipment;

namespace AirBladderUpgrades.Items.Capacity_Upgrades
{
    public class AirBladderCapacityUpgradeMk2
    {
        public static CustomPrefab mk2capacityprefab;
        public static PrefabInfo mk2capacityprefabinfo;
        public static TechType techType = TechType.VehiclePowerUpgradeModule;
        public static void Register()
        {
            mk2capacityprefabinfo = PrefabInfo.WithTechType("AirBladderCapacityUpgradeMk2", "Air Bladder Capacity Upgrade Mk 2", "Mk 2 Capacity for the Air Bladder. Multiples the Oxygen Capacity of the Air Bladder by 4x.").WithIcon(SpriteManager.Get(TechType.AirBladder));
            mk2capacityprefab = new CustomPrefab(mk2capacityprefabinfo);
            var clone = new CloneTemplate(mk2capacityprefabinfo, techType);
            mk2capacityprefab.SetGameObject(clone);
            mk2capacityprefab.SetRecipe(new Nautilus.Crafting.RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(TechType.Battery, 1),
                    new CraftData.Ingredient(TechType.WiringKit, 1)
                }
            })
            .WithFabricatorType(Handheldprefab.HandheldfabTreeType)
            .WithStepsToFabricatorTab("Tools", "AirBladderTab")
            .WithCraftingTime(5f);
            mk2capacityprefab.SetUnlock(TechType.AirBladder);
            mk2capacityprefab.SetPdaGroupCategory(UpgradesLIB.Plugin.toolupgrademodules, Plugin.AirBladderCategory);
            mk2capacityprefab.Register();
            Plugin.Logger.LogInfo("Prefab AirBladderCapacityUpgradeMk2 successfully initialized!");
        }
    }
}
