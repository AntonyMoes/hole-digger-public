using _Game.Scripts.Game.Resource;
using _Game.Scripts.UI.Components.Resource;
using DG.Tweening;
using GeneralUtils;
using UnityEngine;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.UI.Level {
    public class LevelScreenNewItemNotification : MonoBehaviour {
        [SerializeField] private ResourceView _resourceView;
        [SerializeField] private CanvasGroup _canvasGroup;

        private readonly Event _animationEndEvent = new Event();
        public IEvent AnimationEndEvent => _animationEndEvent;

        private Tween _animationTween;

        public void Init(Resource resource) {
            _resourceView.Setup(resource);

            _animationTween?.Kill();

            var rect = (RectTransform) transform;
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;
            rect.anchoredPosition = Vector2.zero;

            const float animationLenght = 0.6f;
            const float verticalShift = 90f;
            _animationTween = DOTween.Sequence()
                .Insert(0f, rect
                    .DOAnchorPosY(verticalShift, animationLenght)
                    .SetEase(Ease.Linear))
                .Insert(0f, _canvasGroup
                    .DOFade(0f, animationLenght)
                    .SetEase(Ease.InCubic))
                .OnComplete(() => {
                    gameObject.SetActive(false);
                    _animationEndEvent.Invoke();
                });
        }
    }
}