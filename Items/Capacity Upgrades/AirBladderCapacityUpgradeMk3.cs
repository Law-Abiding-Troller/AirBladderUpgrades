using System.Collections.Generic;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace AirBladderUpgrades.Items.Capacity_Upgrades
{
    public class AirBladderCapacityUpgradeMk3
    {
        public static CustomPrefab mk3capacityprefab;
        public static PrefabInfo mk3capacityprefabinfo;
        public static TechType techType = TechType.VehiclePowerUpgradeModule;
        public static void Register()
        {
            mk3capacityprefabinfo = PrefabInfo.WithTechType("AirBladderCapacityUpgradeMk3", "Air Bladder Capacity Upgrade Mk 3", "Mk 3 Capacity for the Air Bladder. Increases the amount of Oxygen in the Air Bladder to tripple of Mk 1").WithIcon(SpriteManager.Get(TechType.AirBladder));
            mk3capacityprefab = new CustomPrefab(mk3capacityprefabinfo);
            var clone = new CloneTemplate(mk3capacityprefabinfo, techType);
            mk3capacityprefab.SetGameObject(clone);
            mk3capacityprefab.SetRecipe(new Nautilus.Crafting.RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(TechType.Battery, 1),
                    new CraftData.Ingredient(TechType.WiringKit, 1)
                }
            })
            .WithFabricatorType(CraftTree.Type.Fabricator)
            .WithStepsToFabricatorTab("Personal", "Tools", "AirBladderTab")
            .WithCraftingTime(5f);
            mk3capacityprefab.SetUnlock(TechType.AirBladder);
            mk3capacityprefab.Register();
            Plugin.Logger.LogInfo("Prefab AirBladderCapacityUpgradeMk3 successfully initialized!");
        }
    }
}
