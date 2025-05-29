using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Tutorial;
using _Game.Scripts.DI;
using _Game.Scripts.Scheduling;
using _Game.Scripts.UI;
using _Game.Scripts.UI.Level;
using GeneralUtils;
using UnityEngine;
using Event = GeneralUtils.Event;
using Object = UnityEngine.Object;

namespace _Game.Scripts.Tutorial {
    public class Tutorial {
        private readonly IUIController _uiController;
        private readonly IContainer _container;
        private readonly Parameters _parameters;
        private readonly IWorldPointProvider _worldPointProvider;
        private readonly IUIPointProvider _uiPointProvider;
        private readonly IScheduler _scheduler;

        private readonly List<GameObject> _stepGameObjects = new List<GameObject>();
        private readonly List<IDisposable> _stepSubscriptionTokens = new List<IDisposable>();
        private int _currentStep = -1;

        private readonly Event _finishEvent = new Event();
        public IEvent FinishEvent => _finishEvent;

        [Inject]
        public Tutorial(IUIController uiController, IContainer container, Parameters parameters,
            IWorldPointProvider worldPointProvider, IUIPointProvider uiPointProvider, IScheduler scheduler) {
            _uiController = uiController;
            _container = container;
            _parameters = parameters;
            _worldPointProvider = worldPointProvider;
            _uiPointProvider = uiPointProvider;
            _scheduler = scheduler;
        }

        public void Start() {
            StartNextStep();
        }

        private void Continue() {
            _scheduler.StartCoroutine(ContinueRoutine());

            IEnumerator ContinueRoutine() {
                yield return new WaitForEndOfFrame();
                StartNextStep();
            }
        }

        private void StartNextStep() {
            Clear();

            _currentStep += 1;
            if (_currentStep >= _parameters.Config.Steps.Count) {
                Finish();
                return;
            }

            var step = _parameters.Config.Steps[_currentStep];

            TutorialHider hider = null;
            if (step.Hider is { } hiderData) {
                hider = CreateHider(hiderData);
            }

            if (step.Text is { } text) {
                CreateText(text);
            }

            // place objects

            var initErrorEvent = new Event<string>();
            initErrorEvent.SubscribeOnce(errorMessage =>
                throw new TutorialException(_parameters.Config, _currentStep, errorMessage));

            step.FinishCondition.FinishEvent.SubscribeOnce(Continue);
            step.FinishCondition.Init(new ITutorialStepFinishCondition.Parameters {
                Container = _container,
                Hider = hider,
                InitErrorEvent = initErrorEvent
            });
        }

        private TutorialHider CreateHider(TutorialStep.HiderData hiderData) {
            var hider = _uiController.CreateTutorialObject(_parameters.TutorialHiderPrefab);
            PlaceElement(_parameters.UIView.transform, (RectTransform) hider.transform, hiderData.Position);
            hider.Init(hiderData.ClickThroughHole);
            _stepGameObjects.Add(hider.gameObject);
            return hider;
        }

        private void CreateText(TutorialStep.TextData textData) {
            var text = _uiController.CreateTutorialObject(_parameters.TextPrefab);
            PlaceElement(_parameters.UIView.transform, (RectTransform) text.transform, textData.Position);
            text.Init(textData.Text);
            _stepGameObjects.Add(text.gameObject);
        }

        private void PlaceElement(Transform root, RectTransform element, TutorialStep.PositionData positionData) {
            element.pivot = positionData.Pivot;
            element.anchorMax = positionData.AnchorMax;
            element.anchorMin = positionData.AnchorMin;

            if (positionData.ReferenceElementPath is { } path) {
                var referenceElement = path.Find(root, _container);

                if (referenceElement == null) {
                    throw new TutorialException(_parameters.Config, _currentStep,
                        $"Could not find child of \"{root}\" by path \"{path}\"");
                }

                if (path.Follow) {
                    _stepSubscriptionTokens.Add(_scheduler.FrameEvent.Subscribe(_ =>
                        SetPositionFromReference(root, element, referenceElement, positionData)));
                } else {
                    SetPositionFromReference(root, element, referenceElement, positionData);
                }
            } else {
                element.anchoredPosition = positionData.Position;
                if (positionData.SizeOverride is { } size) {
                    element.sizeDelta = size.UseAsOffset
                        ? element.sizeDelta + size.Value
                        : size.Value;
                }
            }
        }

        private void SetPositionFromReference(Transform root, RectTransform element, Transform referenceElement,
            TutorialStep.PositionData positionData) {
            if (referenceElement == null) {
                return;
            }

            Vector3 referencePosition;
            Vector2 anchoredPositionAdjustment;
            Vector2 referenceSize;
            if (referenceElement is RectTransform referenceRect) {
                referencePosition = referenceRect.position;
                referenceSize = referenceRect.rect.size;
                anchoredPositionAdjustment = (Vector2.one / 2f - referenceRect.pivot) * referenceSize;
            } else {
                var screenPosition = _worldPointProvider.WorldToScreenPoint(referenceElement.position);
                _uiPointProvider.ScreenPointToWorldPointInRectangle(root as RectTransform, screenPosition,
                    out referencePosition);
                anchoredPositionAdjustment = Vector2.zero;
                referenceSize = Vector2.zero;
            }

            element.position = referencePosition;
            element.anchoredPosition += anchoredPositionAdjustment + positionData.Position;
            element.sizeDelta = positionData.SizeOverride is { } size
                ? size.UseAsOffset
                    ? referenceSize + size.Value
                    : size.Value
                : referenceSize;
        }

        private void Clear() {
            foreach (var gameObject in _stepGameObjects) {
                Object.Destroy(gameObject);
            }

            _stepGameObjects.Clear();

            foreach (var token in _stepSubscriptionTokens) {
                token.Dispose();
            }

            _stepSubscriptionTokens.Clear();
        }

        private void Finish() {
            _finishEvent.Invoke();
        }

        private class TutorialException : Exception {
            private readonly TutorialConfig _config;
            private readonly int _step;
            private readonly string _message;

            public TutorialException(TutorialConfig config, int step, string message) {
                _config = config;
                _step = step;
                _message = message;
            }

            public override string Message => $"Tutorial \"{_config.name}\"; Step {_step}; {_message}";
        }

        public struct Parameters {
            public TutorialConfig Config { get; set; }
            public GameObject UIView { get; set; }
            public TutorialHider TutorialHiderPrefab { get; set; }
            public TutorialText TextPrefab { get; set; }
        }
    }
}