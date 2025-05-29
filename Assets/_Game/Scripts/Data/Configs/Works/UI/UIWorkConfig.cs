using _Game.Scripts.DI;
using _Game.Scripts.UI;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Works.UI {
    public abstract class UIWorkConfig<TPresenter, TView, TParameters> : WorkConfig
        where TPresenter : IUIPresenter<TView> where TView : IUIView where TParameters : IUIParameters {
        [SerializeField] protected GameObject UIPrefab;
        [SerializeField] private UILayer _uiLayer;
        public UILayer UILayer => _uiLayer;

        public override IWorkParameters GetParameters() {
            return Parameters;
        }

        protected abstract TParameters Parameters { get; }

        public override bool Do(IContainer container) {
            return Do(container, Parameters);
        }

        public override bool Do(IContainer container, IWorkParameters parameters) {
            if (parameters is not IUIParameters uiParameters) {
                return false;
            }

            var windowManager = container.Get<IUIController>();
            var presenter = container.Create<TPresenter>(parameters);
            return windowManager.Open(uiParameters.UIPrefab, presenter, UILayer, (parameters as ICloseParameters)?.CloseCallback);
        }
    }

    public interface IUIParameters : IWorkParameters {
        GameObject UIPrefab { get; }
    }
}