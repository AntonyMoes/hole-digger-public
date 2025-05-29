using System;
using System.Collections.Generic;
using _Game.Scripts.Game.Resource;
using UnityEngine;

namespace _Game.Scripts.UI.Components.Resource {
    public class ItemResourceView : ResourceView {
        [Header("Value")]
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private ItemResourceViewItem _itemPrefab;

        private readonly List<ItemResourceViewItem> _items = new List<ItemResourceViewItem>();
        private int? _upperLimit;

        protected override void PerformSetup(IResourceHolder holder) {
            _upperLimit = holder.UpperLimit;
        }

        protected override void PerformSetup(Game.Resource.Resource resource) { }

        protected override void SetAmount(int amount) {
            var upperLimit = amount >= 0 ? _upperLimit : null;
            var filledItems = Math.Abs(amount);
            var itemCount = Math.Max(upperLimit ?? 0, filledItems);
            for (var i = 0; i < itemCount; i++) {
                var filled = i < filledItems;
                if (_items.Count == i) {
                    SpawnItem(filled);
                    continue;
                }

                _items[i].SetFilled(filled);
            }

            var itemsToRemove = Math.Max(_items.Count - itemCount, 0);
            for (var i = 0; i < itemsToRemove; i++) {
                DestroyLastItem();
            }
        }

        private void SpawnItem(bool filled) {
            var item = Instantiate(_itemPrefab, _itemContainer);
            item.SetFilled(filled);
            _items.Add(item);
        }

        private void DestroyLastItem() {
            var index = _items.Count - 1;
            Destroy(_items[index].gameObject);
            _items.RemoveAt(index);
        }

        protected override void PerformClear() {
            _upperLimit = null;

            foreach (var item in _items) {
                Destroy(item.gameObject);
            }

            _items.Clear();
        }
    }
}