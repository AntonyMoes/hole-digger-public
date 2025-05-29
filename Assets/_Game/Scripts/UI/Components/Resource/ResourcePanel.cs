using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Game.Resource;
using UnityEngine;

namespace _Game.Scripts.UI.Components.Resource {
    public class ResourcePanel : MonoBehaviour {
        [SerializeField] private ResourceView _resourceView;
        [SerializeField] private ResourceConfig _resource;

        public void Setup(IResourceController resourceController) {
            var resourceHolder = resourceController.GetResource(_resource);
            _resourceView.Setup(resourceHolder);
        }

        private void OnDestroy() {
            _resourceView.Clear();
        }
    }
}