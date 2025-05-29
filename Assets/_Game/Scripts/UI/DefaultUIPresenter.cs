using _Game.Scripts.DI;

namespace _Game.Scripts.UI {
    public class DefaultUIPresenter : UIPresenter<UIView> {
        [Inject]
        public DefaultUIPresenter() { }

        protected override void PerformOpen() { }
        protected override void PerformClose() { }
    }
}