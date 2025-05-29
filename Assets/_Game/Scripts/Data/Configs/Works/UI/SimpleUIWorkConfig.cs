using _Game.Scripts.UI;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Works.UI {
    public abstract class SimpleUIWorkConfig<TPresenter, TView> : UIWorkConfig<TPresenter, TView, SimpleUIParameters>
        where TPresenter : IUIPresenter<TView> where TView : IUIView {
        protected override SimpleUIParameters Parameters => new SimpleUIParameters { UIPrefab = UIPrefab };
    }

    public struct SimpleUIParameters : IUIParameters {
        public GameObject UIPrefab { get; set; }
    }
}