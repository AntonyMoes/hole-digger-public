using System;
using _Game.Scripts.Data.Configs.Meta;
using GeneralUtils;

namespace _Game.Scripts.Game.Resource {
    public interface IResourceHolder {
        public ResourceConfig Config { get; }
        public IUpdatedValue<int> Amount { get; }
        public IUpdatedValue<TimeSpan?> TimeToNextRefill { get; }
        public IUpdatedValue<TimeSpan?> TimeToMax { get; }
        
        public int? UpperLimit { get; }

        public int? InventorySize { get; }

        public Resource Resource => new Resource(Config, Amount.Value);
    }
}