using System;
using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Data.Configs.Meta.Transaction;
using _Game.Scripts.UI.Inventory;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Works.UI {
    [CreateAssetMenu(menuName = Configs.UIWorkMenuItem + nameof(InventoryWindowWorkConfig),
        fileName = nameof(InventoryWindowWorkConfig))]
    public class
        InventoryWindowWorkConfig : UIWorkConfig<InventoryWindowPresenter, IInventoryWindowView,
            InventoryWindowParameters> {
        [SerializeField] private InventoryTransactionConfig[] _resourceSellPrices;
        [SerializeField] private CraftingGroupConfig _craftingGroup;
        [SerializeField] private WorkConfig _setRecipeWork;

        protected override InventoryWindowParameters Parameters => new InventoryWindowParameters {
            UIPrefab = UIPrefab,
            ResourceSellPrices = _resourceSellPrices,
            CraftingGroup = _craftingGroup,
            SetRecipeWork = _setRecipeWork
        };
    }

    public struct InventoryWindowParameters : IUIParameters, ICloseParameters {
        public GameObject UIPrefab { get; set; }
        public IReadOnlyList<InventoryTransactionConfig> ResourceSellPrices { get; set; }
        public CraftingGroupConfig CraftingGroup { get; set; }
        public WorkConfig SetRecipeWork { get; set; }
        public Action CloseCallback { get; set; }
    }
}