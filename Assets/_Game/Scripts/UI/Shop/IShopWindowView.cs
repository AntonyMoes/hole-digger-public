using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Meta.Transaction;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.UI.Components.ResourceLike;
using GeneralUtils;

namespace _Game.Scripts.UI.Shop {
    public interface IShopWindowView : IUIView {
        public IEvent<TransactionConfig> BuyEvent { get; }

        public void InitResourcePanels(IResourceController resourceController);

        public void SetItems(IEnumerable<(TransactionConfig, TransactionResourceLikeData, bool)> items);
    }
}