using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.Game.Resource;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta {
    [CreateAssetMenu(menuName = Configs.MetaMenuItem + nameof(ResourceConfig), fileName = nameof(ResourceConfig))]
    public class ResourceConfig : Config {
        [SerializeField] private Sprite _sprite;
        public Sprite Sprite => _sprite;

        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private string _logName;
        public string LogName => _logName;

        [SerializeReferenceMenu]
        [SerializeReference] private ResourceType.ResourceType _resourceType;
        public IResourceType ResourceType => _resourceType;
    }
}