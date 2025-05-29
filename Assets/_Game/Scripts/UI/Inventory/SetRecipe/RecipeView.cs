using System.Collections.Generic;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Game.Crafting;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.UI.Components;
using _Game.Scripts.UI.Components.ResourceLike;
using GeneralUtils;
using TMPro;
using UnityEngine;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.UI.Inventory.SetRecipe {
    public class RecipeView : MonoBehaviour {
        private const int MinAmount = 1;
        
        [SerializeField] private TextMeshProUGUI _amount;
        [SerializeField] private ResourceLikeView _time;
        [SerializeField] private ResourceLikeView _valuePrefab;
        [SerializeField] private Transform _priceGroup;
        [SerializeField] private Transform _gainGroup;
        [SerializeField] private ActiveElement _subtractButton;
        [SerializeField] private ActiveElement _addButton;
        [SerializeField] private ActiveElement _selectButton;

        private readonly List<ResourceLikeView> _priceValues = new List<ResourceLikeView>();
        private readonly List<ResourceLikeView> _gainValues = new List<ResourceLikeView>();
        private IReadOnlyCrafter _crafter;
        private CraftingConfig _config;
        private UIConfig _uiConfig;
        private IUpdatedValue<int> _amountValue;

        private readonly Event<int> _changeAmountEvent = new Event<int>();
        public IEvent<int> ChangeAmountEvent => _changeAmountEvent;

        private readonly Event _setRecipeEvent = new Event();
        public IEvent SetRecipeEvent => _setRecipeEvent;

        public void Init(IReadOnlyCrafter crafter, UIConfig uiConfig, CraftingConfig config,
            IUpdatedValue<int> amount) {
            _crafter = crafter;
            _uiConfig = uiConfig;
            _config = config;

            _amountValue?.Unsubscribe(SetValues);
            amount.Subscribe(SetValues, true);
            _amountValue = amount;
        }

        private void SetValues(int amount) {
            var price = _config.Price(amount);
            var gain = _config.Reward(amount);
            var time = _config.Time(amount);
            var canCraft = _crafter.CanStartCrafting(_config, amount, out _);

            _amount.SetText(amount.ToString());

            _time.Setup(time.ToResourceLike(_uiConfig));
            SetValueGroup(price, _priceValues, _priceGroup, canCraft);
            SetValueGroup(gain, _gainValues, _gainGroup);

            _selectButton.SetActive(canCraft);

            var canCraftLess = amount > MinAmount && _crafter.CanStartCrafting(_config, amount - 1, out _);
            var canCraftMore = _crafter.CanStartCrafting(_config, amount + 1, out _);
            _subtractButton.SetActive(canCraftLess);
            _addButton.SetActive(canCraftMore);
        }

        private void SetValueGroup(IResourceValue values, List<ResourceLikeView> views, Transform parent,
            bool enough = true) {
            for (var i = 0; i < values.Value.Count; i++) {
                if (views.Count <= i) {
                    views.Add(Instantiate(_valuePrefab, parent));
                }

                var value = values.Value[i];
                var view = views[i];
                view.Setup(value.ToResourceLike(true), enough);
            }

            for (var i = views.Count - 1; i >= values.Value.Count; i--) {
                Destroy(views[i].gameObject);
                views.RemoveAt(i);
            }
        }

        private void OnDestroy() {
            _amountValue.Unsubscribe(SetValues);
        }

        public void ChangeAmount(int delta) {
            _changeAmountEvent.Invoke(delta);
        }

        public void SetRecipe() {
            _setRecipeEvent.Invoke();
        }
    }
}