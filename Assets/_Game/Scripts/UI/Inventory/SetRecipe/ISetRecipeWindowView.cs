using System.Collections.Generic;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Game.Crafting;
using GeneralUtils;

namespace _Game.Scripts.UI.Inventory.SetRecipe {
    public interface ISetRecipeWindowView : IUIView {
        public IEvent<int, int> ChangeAmountEvent { get; }
        public IEvent<int> SetRecipeEvent { get; }

        public void SetRecipes(IReadOnlyCrafter crafter, UIConfig uiConfig, IReadOnlyList<CraftingConfig> recipes,
            IReadOnlyList<IUpdatedValue<int>> amounts);
    }
}