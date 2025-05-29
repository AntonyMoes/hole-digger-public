using System;
using System.Collections.Generic;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Game.Crafting;
using GeneralUtils;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI.Inventory.SetRecipe {
    public class SetRecipeWindowView : UIView, ISetRecipeWindowView {
        [SerializeField] private float _maxContainerHeight;
        [SerializeField] private RecipeView _recipePrefab;
        [SerializeField] private ScrollRect _recipeScroll;
        [SerializeField] private RectTransform _recipeContainer;

        private readonly List<RecipeView> _views = new List<RecipeView>();
        private Dictionary<RecipeView, List<IDisposable>> _viewSubscriptionTokens =
            new Dictionary<RecipeView, List<IDisposable>>();

        private readonly Event<int, int> _changeAmountEvent = new Event<int, int>();
        public IEvent<int, int> ChangeAmountEvent => _changeAmountEvent;

        private readonly Event<int> _setRecipeEvent = new Event<int>();
        public IEvent<int> SetRecipeEvent => _setRecipeEvent;

        public void SetRecipes(IReadOnlyCrafter crafter, UIConfig uiConfig, IReadOnlyList<CraftingConfig> recipes,
            IReadOnlyList<IUpdatedValue<int>> amounts) {
            for (var i = 0; i < recipes.Count; i++) {
                var index = i;
                if (_views.Count <= index) {
                    _views.Add(Instantiate(_recipePrefab, _recipeContainer));
                }

                var recipe = recipes[index];
                var amount = amounts[index];
                var view = _views[index];
                view.Init(crafter, uiConfig, recipe, amount);

                if (!_viewSubscriptionTokens.ContainsKey(view)) {
                    var tokens = new List<IDisposable>();
                    tokens.Add(view.ChangeAmountEvent.Subscribe(delta => _changeAmountEvent.Invoke(index, delta)));
                    tokens.Add(view.SetRecipeEvent.Subscribe(() => _setRecipeEvent.Invoke(index)));
                    _viewSubscriptionTokens.Add(view, tokens);
                }
            }

            for (var i = _views.Count - 1; i >= _views.Count; i--) {
                var view = _views[i];

                foreach (var token in _viewSubscriptionTokens[view]) {
                    token.Dispose();
                }

                _viewSubscriptionTokens.Remove(view);

                Destroy(view.gameObject);
                _views.RemoveAt(i);
            }

            // LayoutRebuilder.ForceRebuildLayoutImmediate(_recipeContainer);
            // TODO set max height
        }
    }
}