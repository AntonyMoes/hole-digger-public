using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.DI;
using GeneralUtils;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Game.Scripts.UI {
    public class UIController : IUIController, IUIPointProvider {
        private readonly UIControllerView _view;

        private readonly Dictionary<UILayer, List<GameObject>> _layerStacks =
            new Dictionary<UILayer, List<GameObject>>();

        private readonly UpdatedValue<GameObject> _activeView = new UpdatedValue<GameObject>();
        public IUpdatedValue<GameObject> ActiveView => _activeView;

        [Inject]
        public UIController(UIControllerView view) {
            _view = view;
        }

        public bool Open<TView>(GameObject uiPrefab, IUIPresenter<TView> presenter, UILayer uiLayer,
            Action closeCallback)
            where TView : IUIView {
            if (!uiPrefab.TryGetComponent(out TView _)) {
                return false;
            }

            var instance = Object.Instantiate(uiPrefab, GetLayerParent(uiLayer));
            var view = instance.GetComponent<TView>();
            view.SetUIPointProvider(this);
            presenter.Open(view, () => OnPresenterClosed(uiLayer, instance, closeCallback));

            GetLayerStack(uiLayer).Add(instance);
            UpdateActiveView();

            return true;
        }

        public TObject CreateTutorialObject<TObject>(TObject prefab) where TObject : Object {
            return Object.Instantiate(prefab, _view.TutorialLevel);
        }

        public bool ScreenPointToWorldPointInRectangle(RectTransform rect, Vector2 screenPosition,
            out Vector3 position) {
            return RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPosition, _view.UICamera,
                out position);
        }

        private void OnPresenterClosed(UILayer layer, GameObject view, [CanBeNull] Action closeCallback) {
            GetLayerStack(layer).Remove(view);
            Object.Destroy(view);

            UpdateActiveView();
            closeCallback?.Invoke();
        }

        private List<GameObject> GetLayerStack(UILayer uiLayer) {
            return _layerStacks.GetValue(uiLayer, () => new List<GameObject>());
        }

        private void UpdateActiveView() {
            _activeView.Value = GetActiveView();
        }

        private GameObject GetActiveView() {
            var layers = Enum.GetValues(typeof(UILayer))
                .Cast<UILayer>()
                .Reverse();
            foreach (var layer in layers) {
                if (!_layerStacks.TryGetValue(layer, out var stack)) {
                    continue;
                }

                if (stack.Count == 0) {
                    continue;
                }

                return stack.Last();
            }

            return null;
        }

        private Transform GetLayerParent(UILayer layer) {
            return layer switch {
                UILayer.Screen => _view.ScreenLayer,
                UILayer.Window => _view.WindowLayer,
                UILayer.Loading => _view.LoadingLayer,
                _ => throw new ArgumentOutOfRangeException(nameof(layer), layer, null)
            };
        }

        public void Dispose() {
            _view.Clear();
        }
    }
}