using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Game.Crafting;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.UI.Components.ResourceLike;
using GeneralUtils;

namespace _Game.Scripts.UI.Inventory {
    public interface IInventoryWindowView : IUIView {
        public IEvent<ResourceConfig> SellResourceEvent { get; }
        public IEvent<int> SetRecipeEvent { get; }
        public IEvent<int> CollectResultEvent { get; }

        public void InitResourcePanels(IResourceController resourceController);

        public void SetSize(int? capacity, int size);

        public void SetItems(IEnumerable<(Resource, TransactionResourceLikeData?, bool)> resources);

        public void SetCraftingSlots(ICraftingGroup craftingGroup, IResourceController resourceController);
    }
}