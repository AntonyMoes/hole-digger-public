using System;
using System.Collections.Generic;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.Game.Resource;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta.ResourceValue {
    [Serializable, SerializeReferenceMenuItem(MenuName = "Config")]
    public class ConfigResourceValue : ResourceValue {
        [SerializeField] private ResourceValueConfig _config;
        public override IReadOnlyList<Resource> Value => _config.Value.Value;
    }
}