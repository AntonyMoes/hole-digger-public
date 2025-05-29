using System;
using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Meta.Transaction;
using _Game.Scripts.UI.Shop;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Works.UI {
    [CreateAssetMenu(menuName = Configs.UIWorkMenuItem + nameof(ShopWindowWorkConfig),
        fileName = nameof(ShopWindowWorkConfig))]
    public class ShopWindowWorkConfig : UIWorkConfig<ShopWindowPresenter, IShopWindowView, ShopWindowParameters> {
        [SerializeField] private TransactionConfig[] _shopItems;

        protected override ShopWindowParameters Parameters => new ShopWindowParameters {
            UIPrefab = UIPrefab,
            ShopItems = _shopItems
        };
    }

    public struct ShopWindowParameters : IUIParameters, ICloseParameters {
        public GameObject UIPrefab { get; set; }
        public IReadOnlyList<TransactionConfig> ShopItems { get; set; }
        public Action CloseCallback { get; set; }
    }
}