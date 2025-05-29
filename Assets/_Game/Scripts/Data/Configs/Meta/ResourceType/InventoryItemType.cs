using System;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.Game.Resource;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta.ResourceType {
    [Serializable, SerializeReferenceMenuItem(MenuName = "InventoryItem")]
    public class InventoryItemType : ResourceType {
        [SerializeField] private int _inventorySize;
        public override int? InventorySize => _inventorySize;

        public override int? UpperLimit => null;
        public override IResourceType.IAutoRefillSettings AutoRefill => null;
    }
}