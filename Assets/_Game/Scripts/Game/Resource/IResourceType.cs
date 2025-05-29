using System;
using JetBrains.Annotations;

namespace _Game.Scripts.Game.Resource {
    public interface IResourceType {
        public int? UpperLimit { get; }
        public int? InventorySize { get; }
        [CanBeNull] public IAutoRefillSettings AutoRefill { get; }

        public interface IAutoRefillSettings {
            public TimeSpan Interval { get; }
            public int Amount { get; }
        }
    }
}