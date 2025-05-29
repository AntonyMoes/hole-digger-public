using System;
using _Game.Scripts.Data.DateTime;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.Game.Resource;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta.ResourceType {
    [Serializable, SerializeReferenceMenuItem(MenuName = "Universal")]
    public class UniversalType : ResourceType {
        [SerializeField] private Optional<int> _upperLimit;
        public override int? UpperLimit => _upperLimit.ToNullable();

        [SerializeField] private Optional<AutoRefillData> _autoRefill;
        public override IResourceType.IAutoRefillSettings AutoRefill => _autoRefill.ToNullableClass();

        public override int? InventorySize => null;

        [Serializable]
        private class AutoRefillData : IResourceType.IAutoRefillSettings {
            [SerializeField] private SerializableTimeSpan _interval;
            public TimeSpan Interval => _interval;

            [SerializeField] private int _amount = 1;
            public int Amount => _amount;
        }
    }
}