using System;
using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Meta.Transaction;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.UI.Components.Resource;
using _Game.Scripts.UI.Components.ResourceLike;
using GeneralUtils;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI.Shop {
    public class ShopWindowView : UIView, IShopWindowView {
        [SerializeField] private ResourcePanel[] _resourcePanels;

        [SerializeField] private int _maxItemsWithoutScroll;
        [SerializeField] private ShopWindowItem _itemPrefab;
        [SerializeField] private RectTransform _itemContainer;
        [SerializeField] private ScrollRect _itemScroll;
        [SerializeField] private GameObject _noMoreContent;

        private readonly List<ShopWindowItem> _items = new List<ShopWindowItem>();
        private readonly List<IDisposable> _subscriptionTokens = new List<IDisposable>();

        private readonly Event<TransactionConfig> _buyEvent = new Event<TransactionConfig>();
        public IEvent<TransactionConfig> BuyEvent => _buyEvent;

        public void InitResourcePanels(IResourceController resourceController) {
            foreach (var resourcePanel in _resourcePanels) {
                resourcePanel.Setup(resourceController);
            }
        }

        public void SetItems(IEnumerable<(TransactionConfig, TransactionResourceLikeData, bool)> items) {
            // can optimize and reuse items
            Clear();

            var itemCount = 0;
            foreach (var (config, transactionData, canPay) in items) {
                var item = Instantiate(_itemPrefab, _itemContainer);
                item.Init(transactionData, canPay);
                _subscriptionTokens.Add(item.BuyEvent.Subscribe(() => _buyEvent.Invoke(config)));
                _items.Add(item);
                itemCount += 1;
            }

            var overMax = itemCount > _maxItemsWithoutScroll;
            _itemScroll.enabled = overMax;
            _itemContainer.anchorMin = _itemContainer.anchorMax = _itemContainer.pivot = !overMax
                ? Vector2.one / 2
                : new Vector2(0, 0.5f);
            _itemContainer.anchoredPosition = Vector2.zero;
            _noMoreContent.SetActive(itemCount == 0);
        }

        private void Clear() {
            foreach (var item in _items) {
                Destroy(item.gameObject);
            }

            _items.Clear();

            foreach (var token in _subscriptionTokens) {
                token.Dispose();
            }

            _subscriptionTokens.Clear();
        }
    }
}