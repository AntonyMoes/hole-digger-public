using System.Collections;
using _Game.Scripts.Data.Configs.Works.UI;
using _Game.Scripts.DI;
using _Game.Scripts.Scheduling;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.UI.Loading {
    public class LoadingWindowPresenter : UIPresenter<IUIView> {
        private readonly IScheduler _scheduler;
        private readonly LoadingWindowParameters _parameters;

        private readonly UpdatedValue<bool> _needToClose = new UpdatedValue<bool>();

        [Inject]
        public LoadingWindowPresenter(IScheduler scheduler, LoadingWindowParameters parameters) {
            _scheduler = scheduler;
            _parameters = parameters;
        }

        protected override void PerformOpen() {
            _parameters.CloseEvent.SubscribeOnce(OnCloseEvent);
            _scheduler.StartCoroutine(Wait());
        }

        private IEnumerator Wait() {
            yield return new WaitForSeconds(_parameters.MinLoadingDuration);
            _needToClose.WaitFor(true, OnNeedToClose);
        }

        private void OnCloseEvent() {
            _needToClose.Value = true;
        }

        private void OnNeedToClose() {
            View.Hide();
            View.ViewState.WaitFor(UI.UIView.State.Hidden, Close);
        }

        protected override void PerformClose() { }
    }
}