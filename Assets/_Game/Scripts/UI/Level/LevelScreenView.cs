using System;
using System.Linq;
using _Game.Scripts.Audio;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.UI.Components.ProgressBar;
using _Game.Scripts.UI.Components.Resource;
using _Game.Scripts.Utils;
using DG.Tweening;
using GeneralUtils;
using TMPro;
using UnityEngine;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.UI.Level {
    public class LevelScreenView : UIView, ILevelScreenView {
        [SerializeField] private ResourcePanel[] _resourcePanels;

        [SerializeField] private SoundConfig _ambient;
        [SerializeField] private TextMeshProUGUI _depth;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private ProgressBar _levelProgress;
        [SerializeField] private GameObject _fullLabel;

        [SerializeField] private RectTransform _fullNotification;
        [SerializeField] private CanvasGroup _fullNotificationCanvasGroup;
        [SerializeField] private SoundConfig _fullSound;

        [SerializeField] private LevelScreenNewItemNotification _newItemNotificationPrefab;
        [SerializeField] private Transform _newItemNotificationParent;

        private readonly Event _openLevelShopEvent = new Event();
        public IEvent OpenLevelShopEvent => _openLevelShopEvent;

        private readonly Event _openInventoryEvent = new Event();
        public IEvent OpenInventoryEvent => _openInventoryEvent;

        private Pool<LevelScreenNewItemNotification> _notificationPool;
        private Tween _cantCollectTween;
        private IAudioHandler _ambientHandler;
        private Tween _startAmbientTween;

        protected override void Init() {
            _notificationPool = new Pool<LevelScreenNewItemNotification>(() =>
                    Instantiate(_newItemNotificationPrefab, _newItemNotificationParent),
                onGet: notification => notification.transform.SetAsLastSibling());
            _ambientHandler = AudioController.Instance.Play(_ambient, true);
            var targetVolume = _ambientHandler.Volume;
            _ambientHandler.Volume = 0f;
            DOVirtual.Float(0, targetVolume, 2f, value => _ambientHandler.Volume = value);
        }

        public void OpenLevelShop() {
            _openLevelShopEvent.Invoke();
        }

        public void OpenInventory() {
            _openInventoryEvent.Invoke();
        }

        public void InitResourcePanels(IResourceController resourceController) {
            foreach (var resourcePanel in _resourcePanels) {
                resourcePanel.Setup(resourceController);
            }
        }

        public void ToggleResourcePanels(bool show) {
            foreach (var resourcePanel in _resourcePanels) {
                resourcePanel.gameObject.SetActive(show);
            }
        }

        public void SetDepth(int depth) {
            _depth.SetText(depth.ToString());
        }

        public void SetExperience(float level) {
            _level.SetText(Mathf.FloorToInt(level).ToString());
            _levelProgress.Progress = level % 1;
        }

        public void SetInventoryFull(bool full) {
            _fullLabel.SetActive(full);
        }

        public void DisplayCollectResource(Resource resource) {
            var handler = _notificationPool.Get();
            handler.Object.AnimationEndEvent.SubscribeOnce(handler.Release);
            handler.Object.Init(resource);
        }

        public void DisplayCantCollectResource(CantAddReason reason, Vector2 screenPosition) {
            if (reason.ReasonType != CantAddReasonType.InventoryFull) {
                throw new Exception(
                    $"Cant collect resources {string.Join(",", reason.Resources.Select(r => r.Name))} with reason {reason.ReasonType}");
            }

            _cantCollectTween?.Kill();
            AudioController.Instance.Play(_fullSound);

            UIPointProvider.ScreenPointToWorldPointInRectangle(transform as RectTransform, screenPosition, out var position);
            _fullNotification.gameObject.SetActive(true);
            _fullNotificationCanvasGroup.alpha = 1f;
            _fullNotification.position = position;

            const float initialVerticalShift = 150f;
            _fullNotification.anchoredPosition += Vector2.up * initialVerticalShift;

            const float animationLenght = 1f;
            const float verticalShift = 150f;
            _cantCollectTween = DOTween.Sequence()
                .Insert(0f, _fullNotification
                    .DOAnchorPosY(_fullNotification.anchoredPosition.y + verticalShift, animationLenght)
                    .SetEase(Ease.Linear))
                .Insert(0f, _fullNotificationCanvasGroup
                    .DOFade(0f, animationLenght)
                    .SetEase(Ease.OutSine))
                .OnComplete(() => _fullNotification.gameObject.SetActive(false));
        }

        private void OnDestroy() {
            _cantCollectTween?.Kill();
            _ambientHandler.Stop();
            _startAmbientTween?.Kill();
        }
    }
}