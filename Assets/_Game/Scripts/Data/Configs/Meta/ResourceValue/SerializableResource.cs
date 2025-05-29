using System;
using _Game.Scripts.Game.Resource;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta.ResourceValue {
    [Serializable]
    public class SerializableResource {
        [SerializeField] private ResourceConfig _resourceConfig;
        public ResourceConfig ResourceConfig => _resourceConfig;

        [SerializeField] private int _amount;
        public int Amount => _amount;

        public static implicit operator Resource(SerializableResource value) {
            return new Resource(value.ResourceConfig, value.Amount);
        }

        public static implicit operator SerializableResource(Resource value) {
            return new SerializableResource {
                _resourceConfig = value.Config,
                _amount = value.Amount
            };
        }
    }
}