using System;
using _Game.Scripts.Analytics;
using _Game.Scripts.Audio;
using _Game.Scripts.Condition;
using _Game.Scripts.Data;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.Data.Configs.Works;
using _Game.Scripts.Data.Configs.Works.UI;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Crafting;
using _Game.Scripts.Game.Level;
using _Game.Scripts.Game.Level.Digging.Tools;
using _Game.Scripts.Game.Leveling;
using _Game.Scripts.Game.Price;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.GameAnalytics;
using _Game.Scripts.Input;
using _Game.Scripts.Scheduling;
using _Game.Scripts.Time;
using _Game.Scripts.Tutorial;
using _Game.Scripts.UI;
using _Game.Scripts.UI.Level;
using _Game.Scripts.Vibration;
using GeneralUtils;
using UnityEngine.EventSystems;

namespace _Game.Scripts.Game {
    public class GameLoader {
        private readonly Func<ApplicationContainer> _createContainer;
        private readonly GameConfig _gameConfig;
        private readonly EventSystem _eventSystem;
        private readonly AudioAdapter _audioAdapter;

        private ApplicationContainer _container;

        public GameLoader(GameConfig gameConfig, Scheduler scheduler, EventSystem eventSystem,
            UIControllerView uiControllerView, AudioAdapter audioAdapter) {
            _gameConfig = gameConfig;
            _eventSystem = eventSystem;
            _audioAdapter = audioAdapter;

            _createContainer = () => {
                var container = new ApplicationContainer(true);
                container.AddInstance(gameConfig);
                container.AddInstance(gameConfig.UIConfig);
                container.AddInstance(gameConfig.LevelConfig);
                container.AddInstance(gameConfig.InventoryConfig);
                container.AddInstance(gameConfig.ConditionsConfig);
                container.AddInstance(gameConfig.AnalyticsConfig);
                container.AddInstance(gameConfig.TutorialsConfig);
                container.AddInstance<IScheduler>(scheduler);
                container.AddInstance<ICoroutineRunner>(scheduler);
                container.AddInstance<ITimeEventProvider>(scheduler);
                var uiController = container.CreateInstance<IUIController, UIController>(uiControllerView);
                container.AddInstance<IUIPointProvider>(uiController);
                return container;
            };
        }

        public void Start() {
            _container = _createContainer();

            _container.Create<VibrationController>();
            _container.Create<AudioController>(_audioAdapter);

            var worldCameraController = _container.CreateInstance<IWorldCameraController, WorldCameraController>();
            _container.AddInstance<IWorldPointProvider>(worldCameraController);

            _container.AddType<ITimeProvider, LocalTimeProvider>();
            _container.AddType<IDataStorage, DataStorage>();
            _container.AddType<ITransactionController, TransactionController>();

            var inputController = InputFactory.CreateController(_eventSystem, _gameConfig.UIConfig.UIMask, _container);
            _container.AddInstance(inputController);

            _container.AddType<IResourceController, ResourceController>();
            _container.AddType<ICraftingController, CraftingController>();
            _container.AddType<ILevelingController, LevelingController>();
            _container.AddType<ILevelController, LevelController>();
            _container.AddType<IToolController, ToolController>();
            _container.AddType<IConditionDataController, ConditionDataController>();

            var analyticsController =
                AnalyticsControllerFactory.CreateController(_gameConfig.AnalyticsConfig, _container);
            _container.AddInstance<IAnalyticsLogger>(analyticsController);

            var tutorialController = _container.Create<TutorialController>();
            _container.AddInstance<ITutorialController>(tutorialController);
            tutorialController.Init();

            _container.Create<GameAnalyticsController>();

            var loadingFinishedEvent = new Event();
            _gameConfig.LoadingWork.TryDoWithParameters(_container, (ref LoadingWindowParameters parameters) =>
                parameters.CloseEvent = loadingFinishedEvent);

            analyticsController.Init().Run(() => {
                loadingFinishedEvent.Invoke();
                _gameConfig.EntryWork.Do(_container);
            });
        }

        private void Restart() {
            Clear();
            Start();
        }

        private void Clear() {
            VibrationController.Instance?.Dispose();
            AudioController.Instance?.Dispose();

            if (_container.TryGet(out IUIController uiController)) {
                uiController.Dispose();
            }

            if (_container.TryGet(out IScheduler scheduler)) {
                scheduler.Dispose();
            }

            _container.Dispose();
        }
    }
}