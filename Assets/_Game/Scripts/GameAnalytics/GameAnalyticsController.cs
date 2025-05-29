using _Game.Scripts.Analytics;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Data.Configs.Tutorial;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Crafting;
using _Game.Scripts.Game.Level;
using _Game.Scripts.Game.Leveling;
using _Game.Scripts.Game.Price;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.GameAnalytics.Events;
using _Game.Scripts.Tutorial;

namespace _Game.Scripts.GameAnalytics {
    public class GameAnalyticsController {
        private readonly AnalyticsConfig _config;
        private readonly IAnalyticsLogger _logger;
        private readonly PriceRewardLogger _priceRewardLogger;

        [Inject]
        public GameAnalyticsController(AnalyticsConfig config, ITransactionController transactionController,
            ICraftingController craftingController, ILevelController levelController,
            ITutorialController tutorialController, IAnalyticsLogger logger, IContainer container) {
            _config = config;
            _logger = logger;
            _priceRewardLogger = container.Create<PriceRewardLogger>();

            transactionController.TransactionPerformEvent.Subscribe(OnPerformTransaction);

            var craftingGroup = craftingController.GetCraftingGroup(config.LoggedCraftingGroupConfig);
            foreach (var crafter in craftingGroup.Crafters) {
                container.Create<CrafterLogger>(crafter, _priceRewardLogger);
            }

            levelController.Depth.Subscribe(OnDepthChange);
            levelController.CollectDropEvent.Subscribe(OnCollectDrop);

            tutorialController.TutorialStartEvent.Subscribe(OnTutorialStart);
            tutorialController.TutorialCompleteEvent.Subscribe(OnTutorialComplete);
        }

        private void OnPerformTransaction(Transaction transaction) {
            _priceRewardLogger.Reason = transaction.Config.LogReason;
            transaction.Price.Log(_priceRewardLogger);
            transaction.Reward.Log(_priceRewardLogger);
        }

        private void OnDepthChange(int depth) {
            if (depth % _config.LevelLoggingFrequency == 0) {
                _logger.Log(new DepthEvent(depth));
            }
        }

        private void OnCollectDrop(IResourceValue drop) {
            _priceRewardLogger.Reason = ResourceEvent.DropReason;
            _priceRewardLogger.LogResource(drop);
        }

        private void OnTutorialStart(TutorialConfig tutorial) {
            _logger.Log(new TutorialEvent(tutorial.Name, TutorialEvent.TutorialState.Start));
        }

        private void OnTutorialComplete(TutorialConfig tutorial) {
            _logger.Log(new TutorialEvent(tutorial.Name, TutorialEvent.TutorialState.Complete));
        }

        private class PriceRewardLogger : IPriceLogger, IRewardLogger {
            private readonly IAnalyticsLogger _logger;
            private readonly ILevelingController _levelingController;

            public string Reason;

            [Inject]
            public PriceRewardLogger(IAnalyticsLogger logger, ILevelingController levelingController) {
                _logger = logger;
                _levelingController = levelingController;
            }

            public void LogResource(IResourceValue value) {
                foreach (var resource in value.Value) {
                    _logger.Log(new ResourceEvent(resource.Amount, resource.Config.LogName, Reason));
                }
            }

            public void LogLevel(LeveledEntityConfig config) {
                var level = _levelingController.GetLevelData(config).Level.Value;
                _logger.Log(new LevelEvent(level, config.LogName));
            }
        }

        private class CrafterLogger {
            private readonly IReadOnlyCrafter _crafter;
            private readonly PriceRewardLogger _logger;

            private CraftingConfig _currentProcess;

            [Inject]
            public CrafterLogger(IReadOnlyCrafter crafter, PriceRewardLogger logger) {
                _crafter = crafter;
                _logger = logger;

                _currentProcess = crafter.CurrentProcess.Value;
                crafter.State.Subscribe(OnStateChange);
            }

            private void OnStateChange(CrafterState state) {
                switch (state) {
                    case CrafterState.Empty when _currentProcess != null:
                        _logger.Reason = ResourceEvent.CraftReason;
                        _logger.LogResource(_currentProcess.Reward);
                        break;
                    case CrafterState.Crafting:
                        _currentProcess = _crafter.CurrentProcess.Value;
                        _logger.Reason = ResourceEvent.CraftReason;
                        _logger.LogResource(_currentProcess.Price);
                        break;
                }
            }
        }
    }
}