using System;
using _Game.Scripts.Data;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.Utils;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.UI.Components.Resource {
    public abstract class ResourceView : MonoBehaviour {
        [Header("Timer")]
        [SerializeField] [CanBeNull] private GameObject _timerGroup;
        [SerializeField] [CanBeNull] private TextMeshProUGUI _timer;
        [SerializeField] [CanBeNull] private GameObject _noTimerGroup;

        [CanBeNull] private IResourceHolder _holder;

        [Header("Animation")]
        [SerializeField] private Optional<float> _amountAnimationLenght;
        [SerializeField] private Ease _amountAnimationEase;

        private int? _animatedAmount;
        private Tween _amountAnimation;

        public void Setup(IResourceHolder holder) {
            Clear();

            PerformSetup(holder);

            _holder = holder;
            _holder.Amount.Subscribe(OnAmountUpdate, true);
            _holder.TimeToNextRefill.Subscribe(OnTimerUpdate, true);
        }

        public void Setup(Game.Resource.Resource resource) {
            Clear();

            PerformSetup(resource);

            SetAmount(resource.Amount);
        }

        protected virtual void PerformSetup(IResourceHolder holder) { }
        protected virtual void PerformSetup(Game.Resource.Resource resource) { }

        public void Clear() {
            _animatedAmount = null;
            _amountAnimation?.Kill();
            _holder?.Amount.Unsubscribe(OnAmountUpdate);
            _holder?.TimeToNextRefill.Unsubscribe(OnTimerUpdate);
            _holder = null;

            PerformClear();
        }

        protected virtual void PerformClear() { }

        private void OnAmountUpdate(int amount) {
            if (_amountAnimationLenght.ToNullable() is not { } animationLength) {
                SetAmount(amount);
                return;
            }
            
            _amountAnimation?.Kill();
            if (_animatedAmount is not { } animatedAmount) {
                AnimationSetAmount(amount);
                return;
            }

            if (animatedAmount == amount) {
                return;
            }

            _amountAnimation = DOVirtual
                .Int(animatedAmount, amount, animationLength, AnimationSetAmount)
                .SetEase(_amountAnimationEase);
        }

        private void AnimationSetAmount(int amount) {
            _animatedAmount = amount;
            SetAmount(amount);
        }

        protected abstract void SetAmount(int amount);

        private void OnTimerUpdate(TimeSpan? timeToRefill) {
            if (_timer == null || timeToRefill is not {} time) {
                if (_timerGroup != null) {
                    _timerGroup.SetActive(false);
                }

                if (_noTimerGroup != null) {
                    _noTimerGroup.SetActive(true);
                }

                return;
            }

            if (_timerGroup != null) {
                _timerGroup.SetActive(true);
            }

            if (_noTimerGroup != null) {
                _noTimerGroup.SetActive(false);
            }

            _timer.text = time.FormatTimer();
        }

        private void OnDestroy() {
            Clear();
        }
    }
}