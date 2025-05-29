using System;
using _Game.Scripts.Data.DateTime;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.Game.Resource;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta {
    [CreateAssetMenu(menuName = Configs.MetaMenuItem + nameof(CraftingConfig), fileName = nameof(CraftingConfig))]
    public class CraftingConfig : Config {
        [SerializeReferenceMenu]
        [SerializeReference] private ResourceValue.ResourceValue _price;
        public IResourceValue Price => _price;

        [SerializeReferenceMenu]
        [SerializeReference] private ResourceValue.ResourceValue _reward;
        public IResourceValue Reward => _reward;

        [SerializeField] private SerializableTimeSpan _time;
        public TimeSpan Time => _time;
    }
}