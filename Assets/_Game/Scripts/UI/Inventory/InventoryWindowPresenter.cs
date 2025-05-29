using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Data.Configs.Meta.ResourceType;
using _Game.Scripts.Data.Configs.Meta.Transaction;
using _Game.Scripts.Data.Configs.Works;
using _Game.Scripts.Data.Configs.Works.UI;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Crafting;
using _Game.Scripts.Game.Price;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.UI.Components.ResourceLike;

namespace _Game.Scripts.UI.Inventory {
    public class InventoryWindowPresenter : UIPresenter<IInventoryWindowView> {
        private readonly IResourceController _resourceController;
        private readonly ITransactionController _transactionController;
        private readonly IContainer _container;
        private readonly InventoryWindowParameters _parameters;
        private readonly ICraftingGroup _craftingGroup;
        private readonly IResourceHolder[] _inventoryResources;
        private readonly Dictionary<ResourceConfig, TransactionConfig> _resourcePrices;
        private readonly List<IDisposable> _subscriptionTokens = new List<IDisposable>();

        [Inject]
        public InventoryWindowPresenter(IResourceController resourceController, ICraftingController craftingController,
            ITransactionController transactionController, IContainer container, InventoryWindowParameters parameters) {
            _resourceController = resourceController;
            _transactionController = transactionController;
            _container = container;
            _parameters = parameters;
            _craftingGroup = craftingController.GetCraftingGroup(parameters.CraftingGroup);
            _inventoryResources = resourceController.Resources
                .Where(resource => resource.ResourceType is InventoryItemType)
                .Select(resourceController.GetResource)
                .ToArray();
            _resourcePrices = parameters.ResourceSellPrices
                .ToDictionary(price => price.InventoryResource, price => (TransactionConfig) price);
        }

        protected override void PerformOpen() {
            View.InitResourcePanels(_resourceController);

            _subscriptionTokens.Add(View.SellResourceEvent.Subscribe(OnSellResource));
            _subscriptionTokens.Add(View.SetRecipeEvent.Subscribe(OnSetRecipe));
            _subscriptionTokens.Add(View.CollectResultEvent.Subscribe(OnCollectResult));
            _subscriptionTokens.Add(_resourceController.InventorySize.Subscribe(_ => SetSize()));
            _subscriptionTokens.Add(_resourceController.InventoryCapacity.Subscribe(_ => SetSize(), true));

            OnResourceUpdate();
            foreach (var holder in _inventoryResources) {
                holder.Amount.Subscribe(OnResourceUpdate);
            }

            View.SetCraftingSlots(_craftingGroup, _resourceController);
        }

        private void OnSellResource(ResourceConfig resource) {
            if (_resourcePrices.TryGetValue(resource, out var transactionConfig) &&
                transactionConfig.TryGetTransaction(_container) is { } transaction) {
                _transactionController.TryPerform(transaction);
            }
        }

        private void OnSetRecipe(int index) {
            var crafter = _craftingGroup.Crafters[index];
            _parameters.SetRecipeWork.TryDoWithParameters(_container, (ref SetRecipeWindowParameters parameters) => {
                parameters.Recipes = _craftingGroup.Recipes;
                parameters.Crafter = crafter;
            });
        }

        private void OnCollectResult(int index) {
            var crafter = _craftingGroup.Crafters[index];
            crafter.TryCollectResult();
        }

        private void SetSize() {
            View.SetSize(_resourceController.InventoryCapacity.Value, _resourceController.InventorySize.Value);
        }

        private void OnResourceUpdate(int _ = 0) {
            var data = _inventoryResources
                .Where(holder => holder.Amount.Value != 0)
                .Select(holder => holder.Resource)
                .Select(resource => {
                    var transaction = _resourcePrices.TryGetValue(resource.Config, out var transactionConfig)
                        ? transactionConfig.TryGetTransaction(_container)
                        : null;
                    var presentation = transaction?.ToResourceLike(_container);
                    var canPay = transaction != null && _transactionController.CanPerform(transaction);
                    return (resource, presentation, canPay);
                });
            View.SetItems(data);
        }

        protected override void PerformClose() {
            foreach (var token in _subscriptionTokens) {
                token.Dispose();
            }

            foreach (var holder in _inventoryResources) {
                holder.Amount.Unsubscribe(OnResourceUpdate);
            }
        }
    }
}