using System.Linq;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.UI.Components.Resource;
using _Game.Scripts.UI.Components.ResourceLike;
using _Game.Scripts.UI.States;
using GeneralUtils;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.UI.Inventory {
    public class InventoryWindowItem : MonoBehaviour {
        [SerializeField] private ResourceView _resourceView;
        [SerializeField] private UIState _empty;
        [SerializeField] private UIState _hasItem;

        [Header("Sell")]
        [SerializeField] private UIState _noPrice;
        [SerializeField] private UIState _canSell;
        [SerializeField] private UIState _cantSell;
        [SerializeField] private ResourceLikeView _priceView;
        [SerializeField] private ResourceLikeView _gainView;

        private ResourceConfig _resourceConfig;

        private readonly Event<ResourceConfig> _sellResourceEvent = new Event<ResourceConfig>();
        public IEvent<ResourceConfig> SellResourceEvent => _sellResourceEvent;

        public void Init() {
            _empty.Apply();
            _noPrice.Apply();
        }

        public void Init(Resource resource, [CanBeNull] TransactionResourceLikeData? transactionData, bool canPay) {
            _resourceConfig = resource.Config;
            _resourceView.Setup(resource);
            _hasItem.Apply();

            if (transactionData is not { } transaction) {
                _noPrice.Apply();
                return;
            }

            _priceView.Setup(transaction.PriceData.First());
            _gainView.Setup(transaction.RewardData.First());

            var state = canPay ? _canSell : _cantSell;
            state.Apply();
        }

        public void Buy() {
            _sellResourceEvent.Invoke(_resourceConfig);
        }
    }
}