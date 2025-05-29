using _Game.Scripts.Game.Resource;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI.Components.Resource {
    public class TextResourceView : ResourceView {
        [Header("Value")]
        [SerializeField] [CanBeNull] private Image _icon;
        [SerializeField] private TextMeshProUGUI _value;

        [SerializeField] private bool _abs = true;
        [SerializeField] private bool _addPlus = false;

        protected override void PerformSetup(IResourceHolder holder) {
            if (_icon != null) {
                _icon.sprite = holder.Config.Sprite;
            }
        }

        protected override void PerformSetup(Game.Resource.Resource resource) {
            if (_icon != null) {
                _icon.sprite = resource.Config.Sprite;
            }
        }

        protected override void SetAmount(int amount) {
            var value = _abs ? Mathf.Abs(amount) : amount;
            _value.text = (_addPlus && amount > 0 ? "+" : "") + value;
        }
    }
}