using System;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.UI.Loading {
    public class LoadingWindowView : UIView {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDelay;
        [SerializeField] private float _fadeDuration;

        protected override void PerformShow(Action onDone = null) {
            _canvasGroup.alpha = 1f;
            onDone?.Invoke();
        }

        protected override void PerformHide(Action onDone = null) {
            DOTween.Sequence()
                .AppendInterval(_fadeDelay)
                .Append(_canvasGroup.DOFade(0f, _fadeDuration)
                    .OnComplete(() => onDone?.Invoke()));
        }
    }
}