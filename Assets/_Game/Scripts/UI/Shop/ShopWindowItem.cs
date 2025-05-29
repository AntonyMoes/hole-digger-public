using System.Linq;
using _Game.Scripts.UI.Components;
using _Game.Scripts.UI.Components.ResourceLike;
using GeneralUtils;
using UnityEngine;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.UI.Shop {
    public class ShopWindowItem : MonoBehaviour {
        [SerializeField] private ActiveElement _buyButton;
        [SerializeField] private ResourceLikeView _reward;
        [SerializeField] private ResourceLikeView _price;

        private readonly Event _buyEvent = new Event();
        public IEvent BuyEvent => _buyEvent;

        public void Init(TransactionResourceLikeData transactionData, bool canPay) {
            _reward.Setup(transactionData.RewardData.First());
            _price.Setup(transactionData.PriceData.First());
            _buyButton.SetActive(canPay);
        }

        public void Buy() {
            _buyEvent.Invoke();
        }
    }
}