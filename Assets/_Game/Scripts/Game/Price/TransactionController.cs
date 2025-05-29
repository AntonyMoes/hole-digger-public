using _Game.Scripts.Data.Configs;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Leveling;
using _Game.Scripts.Game.Resource;
using GeneralUtils;

namespace _Game.Scripts.Game.Price {
    public class TransactionController : ITransactionController {
        private readonly IContainer _container;
        private readonly IPriceProcessor _priceProcessor;
        private readonly IRewardProcessor _rewardProcessor;

        private readonly Event<Transaction> _transactionPerformEvent = new Event<Transaction>();
        public IEvent<Transaction> TransactionPerformEvent => _transactionPerformEvent;

        [Inject]
        public TransactionController(IContainer container) {
            _container = container;
            _priceProcessor = container.Create<PriceProcessor>();
            _rewardProcessor = container.Create<RewardProcessor>();
        }

        public bool CanPerform(Transaction transaction) {
            return transaction.Price.CanPay(_priceProcessor, _container) &&
                   transaction.Reward.CanAdd(_rewardProcessor, _container);
        }

        public bool TryPerform(Transaction transaction) {
            if (!CanPerform(transaction)) {
                return false;
            }

            transaction.Price.TryPay(_priceProcessor, _container);
            transaction.Reward.TryAdd(_rewardProcessor, _container);
            _transactionPerformEvent.Invoke(transaction);
            return true;
        }

        private class PriceProcessor : IPriceProcessor {
            private readonly IResourceController _resourceController;

            [Inject]
            public PriceProcessor(IResourceController resourceController) {
                _resourceController = resourceController;
            }

            public bool CanPayResource(IResourceValue value) {
                return _resourceController.CanAdd(value);
            }

            public bool TryPayResource(IResourceValue value) {
                return _resourceController.TryAdd(value);
            }
        }

        private class RewardProcessor : IRewardProcessor {
            private readonly IResourceController _resourceController;
            private readonly ILevelingController _levelingController;

            [Inject]
            public RewardProcessor(IResourceController resourceController, ILevelingController levelingController) {
                _resourceController = resourceController;
                _levelingController = levelingController;
            }

            public bool CanAddResource(IResourceValue value) {
                return _resourceController.CanAdd(value);
            }

            public bool TryAddResource(IResourceValue value) {
                return _resourceController.TryAdd(value);
            }

            public bool CanAddLevel(LeveledEntityConfig config) {
                return _levelingController.CanAddLevel(config);
            }

            public bool TryAddLevel(LeveledEntityConfig config) {
                return _levelingController.TryAddLevel(config);
            }
        }
    }
}