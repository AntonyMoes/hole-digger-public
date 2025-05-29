using _Game.Scripts.Game.Resource;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta.ResourceValue {
    [CreateAssetMenu(menuName = Configs.MetaMenuItem + nameof(ResourceValueConfig), fileName = nameof(ResourceValueConfig))]
    public class ResourceValueConfig : Config {
        [SerializeField] private InPlaceResourceValue _value;
        public IResourceValue Value => _value;
    }
}