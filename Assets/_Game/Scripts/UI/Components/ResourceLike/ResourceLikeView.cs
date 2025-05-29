using _Game.Scripts.UI.States;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI.Components.ResourceLike {
    public class ResourceLikeView : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _amount;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Image _icon;
        [SerializeField] private UIState _enoughState;
        [SerializeField] private UIState _notEnoughState;

        public void Setup(ResourceLikeData data, bool enough = true) {
            if (_name != null) {
                _name.SetText(data.Name);
            }

            if (_amount != null) {
                _amount.SetText(data.Amount);
            }

            if (_description != null) {
                _description.SetText(data.Description);
            }

            if (_icon != null) {
                _icon.sprite = data.Icon;
            }

            var state = enough ? _enoughState : _notEnoughState;
            state.Apply();
        }
    }
}