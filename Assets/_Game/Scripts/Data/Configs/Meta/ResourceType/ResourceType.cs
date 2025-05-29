using System;
using _Game.Scripts.Game.Resource;

namespace _Game.Scripts.Data.Configs.Meta.ResourceType {
    [Serializable]
    public abstract class ResourceType : IResourceType {
        public abstract int? UpperLimit { get; }
        public abstract int? InventorySize { get; }
        public abstract IResourceType.IAutoRefillSettings AutoRefill { get; }
    }
}