using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Game.Scripts.Data {
    [Serializable]
    public class ListData<TItem> where TItem : class, ListData<TItem>.IItem, new() {
        [SerializeField] private List<TItem> _items = new List<TItem>();
        public IReadOnlyList<TItem> Items => _items;

        public TItem GetItem(int configId) {
            var item = _items.FirstOrDefault(i => i.ConfigId == configId);
            if (item == null) {
                item = new TItem();
                item.ConfigId = configId;
                _items.Add(item);
            }

            return item;
        }
        
        public interface IItem {
            public int ConfigId { get; set; }
        }
    }
}