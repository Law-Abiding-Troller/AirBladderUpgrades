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

namespace AirBladderUpgrades.Items.Capacity_Upgrades
{
    public class AirBladderCapacityUpgradeMk1 //all of these are nearly the same, just some name changes and very little behavior. let me know if i should change mk1, mk2, or 3.
    {
        public static CustomPrefab mk1capacityprefab;
        public static PrefabInfo mk1capacityprefabinfo;
        public static TechType techType = TechType.VehiclePowerUpgradeModule;
        public static void Register()
        {
            mk1capacityprefabinfo = PrefabInfo.WithTechType("AirBladderCapacityUpgradeMk1", "Air Bladder Capacity Upgrade Mk 1", "Mk 1 Capacity for the Air Bladder. Increases the amount of Oxygen in the Air Bladder to 30 seconds").WithIcon(SpriteManager.Get(TechType.AirBladder));
            mk1capacityprefab = new CustomPrefab(mk1capacityprefabinfo);
            var clone = new CloneTemplate(mk1capacityprefabinfo, techType);
            clone.ModifyPrefab += obj =>
            {
                GameObject model = obj.gameObject;
                model.transform.localScale = Vector3.one / (1 + 1 / 2);
            };
            mk1capacityprefab.SetGameObject(clone);
            mk1capacityprefab.SetRecipe(new Nautilus.Crafting.RecipeData()
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
            mk1capacityprefab.SetUnlock(TechType.AirBladder);
            mk1capacityprefab.Register();
            Plugin.Logger.LogInfo("Prefab AirBladderCapacityUpgradeMk1 successfully initialized!");
        }
    }
}
