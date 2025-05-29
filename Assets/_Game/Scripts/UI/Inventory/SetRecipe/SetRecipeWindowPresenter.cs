using System;
using System.Linq;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.Data.Configs.Works.UI;
using _Game.Scripts.DI;
using GeneralUtils;

namespace _Game.Scripts.UI.Inventory.SetRecipe {
    public class SetRecipeWindowPresenter : UIPresenter<ISetRecipeWindowView> {
        private const int MinAmount = 1;

        private readonly UIConfig _uiConfig;
        private readonly SetRecipeWindowParameters _parameters;
        private readonly UpdatedValue<int>[] _amounts;

        [Inject]
        public SetRecipeWindowPresenter(UIConfig uiConfig, SetRecipeWindowParameters parameters) {
            _uiConfig = uiConfig;
            _parameters = parameters;
            _amounts = Enumerable
                .Range(0, parameters.Recipes.Count)
                .Select(_ => new UpdatedValue<int>(MinAmount, SetAmount))
                .ToArray();
        }

        protected override void PerformOpen() {
            View.ChangeAmountEvent.Subscribe(OnChangeAmount);
            View.SetRecipeEvent.Subscribe(OnSetRecipe);
            View.SetRecipes(_parameters.Crafter, _uiConfig, _parameters.Recipes, _amounts);
        }

        private void OnChangeAmount(int index, int delta) {
            _amounts[index].Value += delta;
        }

        private void OnSetRecipe(int index) {
            var recipe = _parameters.Recipes[index];
            var amount = _amounts[index];
            if (_parameters.Crafter.TryStartCrafting(recipe, amount.Value, out _)) {
                Close();
            }
        }

        protected override void PerformClose() {
            View.ChangeAmountEvent.Unsubscribe(OnChangeAmount);
            View.SetRecipeEvent.Unsubscribe(OnSetRecipe);
        }

        private static int SetAmount(int value) {
            return Math.Max(MinAmount, value);
        }
    }
}