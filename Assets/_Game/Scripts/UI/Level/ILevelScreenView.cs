using _Game.Scripts.Game.Resource;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.UI.Level {
    public interface ILevelScreenView : IUIView {
        public IEvent OpenLevelShopEvent { get; }
        public IEvent OpenInventoryEvent { get; }

        public void InitResourcePanels(IResourceController resourceController);
        public void ToggleResourcePanels(bool show);

        public void SetDepth(int depth);
        public void SetExperience(float level);

        public void SetInventoryFull(bool full);
        public void DisplayCollectResource(Resource resource);

        public void DisplayCantCollectResource(CantAddReason reason, Vector2 screenPosition);
    }
}