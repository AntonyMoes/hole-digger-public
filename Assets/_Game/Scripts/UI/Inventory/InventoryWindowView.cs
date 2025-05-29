using System;
using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Game.Crafting;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.UI.Components.Resource;
using _Game.Scripts.UI.Components.ResourceLike;
using GeneralUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI.Inventory {
    public class InventoryWindowView : UIView, IInventoryWindowView {
        [SerializeField] private ResourcePanel[] _resourcePanels;

        [Header("Items")]
        [SerializeField] private int _maxItemsWithoutScroll;
        [SerializeField] private TextMeshProUGUI _size;
        [SerializeField] private InventoryWindowItem _itemPrefab;
        [SerializeField] private ScrollRect _itemScroll;
        [SerializeField] private RectTransform _itemContainer;

        [Header("Melting")]
        [SerializeField] private int _maxSlotsWithoutScroll;
        [SerializeField] private InventoryWindowCraftingSlot _slotPrefab;
        [SerializeField] private ScrollRect _slotScroll;
        [SerializeField] private RectTransform _slotContainer;

        private readonly List<InventoryWindowItem> _items = new List<InventoryWindowItem>();
        private readonly List<InventoryWindowCraftingSlot> _slots = new List<InventoryWindowCraftingSlot>();
        private readonly List<IDisposable> _slotSubscriptionTokens = new List<IDisposable>();

        private readonly Event<ResourceConfig> _sellResourceEvent = new Event<ResourceConfig>();
        public IEvent<ResourceConfig> SellResourceEvent => _sellResourceEvent;

        private readonly Event<int> _setRecipeEvent = new Event<int>();
        public IEvent<int> SetRecipeEvent => _setRecipeEvent;

        private readonly Event<int> _collectResultEvent = new Event<int>();
        public IEvent<int> CollectResultEvent => _collectResultEvent;

        public void InitResourcePanels(IResourceController resourceController) {
            foreach (var resourcePanel in _resourcePanels) {
                resourcePanel.Setup(resourceController);
            }
        }

        public void SetSize(int? capacity, int size) {
            var text = capacity is { } cap
                ? $"{size}/{capacity}"
                : size.ToString();
            _size.SetText(text);
        }

        public void SetItems(IEnumerable<(Resource, TransactionResourceLikeData?, bool)> resources) {
            // can optimize and reuse items
            ClearItems();

            var itemCount = 0;
            foreach (var (resource, transactionData, canPay) in resources) {
                var item = Instantiate(_itemPrefab, _itemContainer);
                item.Init(resource, transactionData, canPay);
                item.SellResourceEvent.Subscribe(_sellResourceEvent.Invoke);
                _items.Add(item);
                itemCount += 1;
            }

            var overMax = itemCount > _maxItemsWithoutScroll;
            _itemScroll.enabled = overMax;
            if (!overMax) {
                _itemContainer.anchoredPosition = Vector2.zero;
                for (var i = itemCount; i < _maxItemsWithoutScroll; i++) {
                    var item = Instantiate(_itemPrefab, _itemContainer);
                    item.Init();
                    _items.Add(item);
                }
            }
        }

        public void SetCraftingSlots(ICraftingGroup craftingGroup, IResourceController resourceController) {
            ClearSlots();

            for (var i = 0; i < craftingGroup.Crafters.Count; i++) {
                var index = i;
                var crafter = craftingGroup.Crafters[index];
                var slot = Instantiate(_slotPrefab, _slotContainer);
                _slots.Add(slot);
                slot.Init(crafter, resourceController);
                _slotSubscriptionTokens.Add(slot.SetRecipeEvent.Subscribe(() => _setRecipeEvent.Invoke(index)));
                _slotSubscriptionTokens.Add(slot.CollectResultEvent.Subscribe(() => _collectResultEvent.Invoke(index)));
            }

            var overMax = craftingGroup.Crafters.Count > _maxSlotsWithoutScroll;
            _slotScroll.enabled = overMax;
            if (!overMax) {
                _slotContainer.anchoredPosition = Vector2.zero;
            }
        }

        private void ClearItems() {
            foreach (var item in _items) {
                item.SellResourceEvent.Unsubscribe(_sellResourceEvent.Invoke);
                Destroy(item.gameObject);
            }

            _items.Clear();
        }

        private void ClearSlots() {
            foreach (var slot in _slots) {
                Destroy(slot.gameObject);
            }

            foreach (var token in _slotSubscriptionTokens) {
                token.Dispose();
            }

            _slots.Clear();
            _slotSubscriptionTokens.Clear();
        }
    }
}