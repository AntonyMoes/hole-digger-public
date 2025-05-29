using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Game.Crafting;
using _Game.Scripts.UI.Inventory.SetRecipe;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Works.UI {
    [CreateAssetMenu(menuName = Configs.UIWorkMenuItem + nameof(SetRecipeWindowWorkConfig),
        fileName = nameof(SetRecipeWindowWorkConfig))]
    public class
        SetRecipeWindowWorkConfig : UIWorkConfig<SetRecipeWindowPresenter, ISetRecipeWindowView,
            SetRecipeWindowParameters> {
        protected override SetRecipeWindowParameters Parameters => new SetRecipeWindowParameters {
            UIPrefab = UIPrefab,
        };
    }

    public struct SetRecipeWindowParameters : IUIParameters {
        public GameObject UIPrefab { get; set; }
        public IReadOnlyList<CraftingConfig> Recipes { get; set; }
        public ICrafter Crafter { get; set; }
    }
}