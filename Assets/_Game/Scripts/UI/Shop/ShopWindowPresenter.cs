using System.Linq;
using _Game.Scripts.Data.Configs.Meta.Transaction;
using _Game.Scripts.Data.Configs.Works.UI;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Price;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.UI.Components.ResourceLike;

namespace _Game.Scripts.UI.Shop {
    public class ShopWindowPresenter : UIPresenter<IShopWindowView> {
        private readonly ITransactionController _transactionController;
        private readonly IResourceController _resourceController;
        private readonly IContainer _container;
        private readonly ShopWindowParameters _parameters;

        [Inject]
        public ShopWindowPresenter(ITransactionController transactionController, IResourceController resourceController,
            IContainer container, ShopWindowParameters parameters) {
            _transactionController = transactionController;
            _resourceController = resourceController;
            _container = container;
            _parameters = parameters;
        }

        protected override void PerformOpen() {
            View.InitResourcePanels(_resourceController);
            View.BuyEvent.Subscribe(OnBuy);
            UpdateItems();
        }

        private void UpdateItems() {
            View.SetItems(_parameters.ShopItems
                .Select(config => (config, config.TryGetTransaction(_container)))
                .Where(pair => pair.Item2 != null)
                .Select(pair => {
                    var transaction = pair.Item2;
                    var presentation = transaction.ToResourceLike(_container);
                    var canPay = _transactionController.CanPerform(transaction);
                    return (pair.config, presentation, canPay);
                }));
        }

        private void OnBuy(TransactionConfig config) {
            var transaction = config.TryGetTransaction(_container);
            if (transaction == null || !_transactionController.TryPerform(transaction)) {
                return;
            }

            UpdateItems();
        }

        protected override void PerformClose() {
            View.BuyEvent.Unsubscribe(OnBuy);
        }
    }
}