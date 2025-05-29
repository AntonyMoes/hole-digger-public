using System;
using GeneralUtils;
using UnityEngine;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.UI {
    public class UIView : MonoBehaviour, IUIView {
        private readonly Event _closeEvent = new Event();
        public IEvent CloseEvent => _closeEvent;

        public void Close() {
            _closeEvent.Invoke();
        }

        private readonly UpdatedValue<State> _state = new UpdatedValue<State>(State.Hidden);
        public IUpdatedValue<State> ViewState => _state;

        private CanvasGroup _group;

        protected virtual bool ChangeInteractivity => true;

        private bool _locked;
        public bool Locked {
            get => _locked;
            set {
                _locked = value;
                SetGroupInteractable(value, Interactable);
            }
        }

        private bool _interactable;
        private bool Interactable {
            get => _interactable;
            set {
                _interactable = value;
                SetGroupInteractable(Locked, Interactable);
            }
        }

        protected IUIPointProvider UIPointProvider { get; private set; }

        private void Awake() {
            Init();
            _group = TryGetComponent<CanvasGroup>(out var group) ? group : gameObject.AddComponent<CanvasGroup>();
        }

        private void SetGroupInteractable(bool locked, bool interactable) {
            var groupInteractable = !locked && interactable;
            if (ChangeInteractivity) {
                _group.interactable = groupInteractable;
            } else {
                _group.blocksRaycasts = groupInteractable;
            }
        }

        protected virtual void Init() { }

        public void SetUIPointProvider(IUIPointProvider uiPointProvider) {
            UIPointProvider = uiPointProvider;
        }

        public void Show() {
            if (_state.Value == State.Shown) {
                return;
            }

            if (_state.Value == State.Showing) {
                return;
            }

            // TODO what to do if Hiding?

            _state.Value = State.Showing;
            gameObject.SetActive(true);

            Interactable = false;

            PerformShow(() => {
                Interactable = true;

                _state.Value = State.Shown;
            });
        }

        protected virtual void PerformShow(Action onDone = null) {
            onDone?.Invoke();
        }

        public void Hide() {
            if (_state.Value == State.Hidden) {
                return;
            }

            if (_state.Value == State.Hiding) {
                return;
            }

            // TODO what to do if Showing?

            _state.Value = State.Hiding;

            Interactable = false;

            PerformHide(() => {
                Interactable = true;

                gameObject.SetActive(false);
                _state.Value = State.Hidden;
            });
        }

        protected virtual void PerformHide(Action onDone = null) {
            onDone?.Invoke();
        }

        public enum State {
            Showing,
            Shown,
            Hiding,
            Hidden
        }
    }
}