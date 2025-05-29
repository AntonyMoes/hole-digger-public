using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Condition;
using _Game.Scripts.Data;
using _Game.Scripts.Data.Configs.Tutorial;
using _Game.Scripts.DI;
using _Game.Scripts.Scheduling;
using _Game.Scripts.UI;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Tutorial {
    public class TutorialController : ITutorialController {
        private const string DataKey = "Tutorials";

        private readonly List<TutorialConfig> _tutorials;
        private readonly ListData<TutorialData> _tutorialData;
        private readonly IDataStorage _dataStorage;
        private readonly IUIController _uiController;
        private readonly IContainer _container;
        private readonly IScheduler _scheduler;
        private readonly TutorialHider _hiderPrefab;
        private readonly TutorialText _textPrefab;

        private readonly Dictionary<TutorialConfig, ICondition> _tutorialConditions =
            new Dictionary<TutorialConfig, ICondition>();
        private readonly Dictionary<TutorialConfig, IDisposable> _tutorialConditionSubscriptions =
            new Dictionary<TutorialConfig, IDisposable>();
        private Tutorial _currentTutorial;

        private readonly Event<TutorialConfig> _tutorialStartEvent = new Event<TutorialConfig>();
        public IEvent<TutorialConfig> TutorialStartEvent => _tutorialStartEvent;

        private readonly Event<TutorialConfig> _tutorialCompleteEvent = new Event<TutorialConfig>();
        public IEvent<TutorialConfig> TutorialCompleteEvent => _tutorialCompleteEvent;

        [Inject]
        public TutorialController(TutorialsConfig config, IDataStorage dataStorage, IUIController uiController,
            IContainer container, IScheduler scheduler) {
            _tutorialData = dataStorage.GetData<ListData<TutorialData>>(DataKey);
            _tutorials = config.Tutorials
                .Where(tutorial => tutorial.IgnoreComplete || !_tutorialData.GetItem(tutorial.ConfigId).complete)
                .OrderByDescending(tutorial => tutorial.Priority)
                .ToList();
            _dataStorage = dataStorage;
            _uiController = uiController;
            _container = container;
            _scheduler = scheduler;
            _hiderPrefab = config.TutorialHiderPrefab;
            _textPrefab = config.TutorialTextPrefab;

            uiController.ActiveView.Subscribe(_ => CheckTutorial());
        }

        public void Init() {
            foreach (var tutorial in _tutorials) {
                var condition = tutorial.Condition.GetCondition(_container);
                _tutorialConditions.Add(tutorial, condition);
                _tutorialConditionSubscriptions.Add(tutorial, condition.Value.Subscribe(value => {
                    if (value) {
                        CheckTutorial();
                    }
                }));
            }
        }

        public bool IsComplete(TutorialConfig tutorial) {
            return _tutorialData.GetItem(tutorial.ConfigId).complete;
        }

        private void CheckTutorial(TutorialConfig ignoredTutorial = null) {
            if (_currentTutorial != null || _uiController.ActiveView.Value == null) {
                return;
            }

            foreach (var config in _tutorials) {
                // prevent loops for permanent tutorials
                if (config == ignoredTutorial) {
                    continue;
                }

                if (!_uiController.ActiveView.Value.name.StartsWith(config.UIEntryPoint)) {
                    continue;
                }

                if (!_tutorialConditions[config].Value.Value) {
                    continue;
                }

                StartTutorial(config);
                break;
            }
        }

        private void StartTutorial(TutorialConfig config) {
            if (config.InstantlyMarkComplete) {
                MarkTutorialComplete(config);
            }

            _currentTutorial = _container.Create<Tutorial>(new Tutorial.Parameters {
                Config = config,
                UIView = _uiController.ActiveView.Value,
                TutorialHiderPrefab = _hiderPrefab,
                TextPrefab = _textPrefab
            });
            _currentTutorial.FinishEvent.SubscribeOnce(() => {
                if (!config.InstantlyMarkComplete) {
                    MarkTutorialComplete(config);
                }

                _currentTutorial = null;
                FinishTutorial(config);
            });

            _tutorialStartEvent.Invoke(config);
            _scheduler.StartCoroutine(StartTutorialRoutine());
        }

        private IEnumerator StartTutorialRoutine() {
            yield return new WaitForEndOfFrame();
            _currentTutorial.Start();
        }

        private void MarkTutorialComplete(TutorialConfig config) {
            _tutorialData.GetItem(config.ConfigId).complete = true;
            Save();
            _tutorialCompleteEvent.Invoke(config);
        }

        private void FinishTutorial(TutorialConfig config) {
            if (!config.IgnoreComplete) {
                _tutorials.Remove(config);
                _tutorialConditions.Remove(config);
                _tutorialConditionSubscriptions[config].Dispose();
                _tutorialConditionSubscriptions.Remove(config);
            }

            CheckTutorial(config);
        }

        private void Save() {
            _dataStorage.SetData(_tutorialData, DataKey);
        }

        [Serializable]
        private class TutorialData : ListData<TutorialData>.IItem {
            public int configId;
            public bool complete;

            public int ConfigId {
                get => configId;
                set => configId = value;
            }
        }
    }
}