using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.Game.Resource;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta.ResourceValue {
    [Serializable, SerializeReferenceMenuItem(MenuName = "In place")]
    public class InPlaceResourceValue : ResourceValue {
        [ArrayElementTitle("_resourceConfig")]
        [SerializeField] private SerializableResource[] _values;
        public override IReadOnlyList<Resource> Value => _values
            .Select(v => (Resource) v)
            .ToArray();
        
        
    }
}