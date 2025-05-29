using System;
using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Meta;
using GeneralUtils;

namespace _Game.Scripts.Game.Resource {
    public interface IResourceController : IDisposable {
        public IUpdatedValue<int?> InventoryCapacity { get; }
        public IUpdatedValue<int> InventorySize { get; }

        public IReadOnlyList<ResourceConfig> Resources { get; }
        public IResourceHolder GetResource(ResourceConfig config);

        public bool TryAdd(IResourceValue resources, bool asMuchAsPossible = false);
        public bool TryAdd(IResourceValue resources, out CantAddReason reason, bool asMuchAsPossible = false);
        public bool CanAdd(IResourceValue resources, bool asMuchAsPossible = false);
        public bool CanAdd(IResourceValue resources, out CantAddReason reason, bool asMuchAsPossible = false);
    }
}