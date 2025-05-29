using System;
using System.Collections.Generic;
using _Game.Scripts.Game.Resource;

namespace _Game.Scripts.Data.Configs.Meta.ResourceValue {
    [Serializable]
    public abstract class ResourceValue : IResourceValue {
        public abstract IReadOnlyList<Resource> Value { get; }
    }
}