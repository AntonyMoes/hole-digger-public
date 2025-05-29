using _Game.Scripts.UI;
using _Game.Scripts.UI.Loading;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Works.UI {
    [CreateAssetMenu(menuName = Configs.UIWorkMenuItem + nameof(LoadingWindowWorkConfig),
        fileName = nameof(LoadingWindowWorkConfig))]
    public class LoadingWindowWorkConfig : UIWorkConfig<LoadingWindowPresenter, IUIView, LoadingWindowParameters> {
        [SerializeField] private float _minLoadingDuration;
        
        protected override LoadingWindowParameters Parameters => new LoadingWindowParameters {
            UIPrefab = UIPrefab,
            MinLoadingDuration = _minLoadingDuration
        };
    }

    public struct LoadingWindowParameters : IUIParameters {
        public GameObject UIPrefab { get; set; }
        public float MinLoadingDuration { get; set; }
        public IEvent CloseEvent { get; set; }
    }
}