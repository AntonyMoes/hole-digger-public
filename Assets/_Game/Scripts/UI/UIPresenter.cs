using System;
using _Game.Scripts.Utils;

namespace _Game.Scripts.UI {
    public abstract class UIPresenter<TView> : IUIPresenter<TView> where TView : IUIView {
        private TView _view;
        protected TView View => _view.NullSafe();
        public IUIView UIView => View;

        private Action _closeCallback;
        private bool _closing;

        public virtual void Back() {
            Close();
        }

        public void Open(TView view, Action closeCallback) {
            _view = view;
            _closeCallback = closeCallback;

            if (View != null) {
                View.CloseEvent.Subscribe(Close);
                Show();
            }

            PerformOpen();
        }

        protected abstract void PerformOpen();

        public void Close() {
            if (_closing) {
                return;
            }

            _closing = true;
            if (View != null) {
                View.CloseEvent.Unsubscribe(Close);
                Hide();

                PerformClose();
                View.ViewState.WaitFor(UI.UIView.State.Hidden, FinishClosing);
            } else {
                PerformClose();
                FinishClosing();
            }
        }

        private void FinishClosing() {
            _view = default;

            _closeCallback?.Invoke();
            _closeCallback = null;
        }

        protected abstract void PerformClose();

        public void Hide() => View?.Hide();

        public void Show() => View?.Show();
    }
}