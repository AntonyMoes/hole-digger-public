using _Game.Scripts.Vibration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.Debug {
    public class VibrationTestWidget : MonoBehaviour {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;

        [Header("Settings")]
        [SerializeField] private VibrationType[] _targets;

        private int _currentTarget = 0;

        private void Awake() {
            _button.onClick.AddListener(OnClick);
            _text.text = _targets[_currentTarget].ToString();
        }

        private void OnClick() {
            VibrationController.Instance.Vibrate(_targets[_currentTarget]);

            _currentTarget = (_currentTarget + 1) % _targets.Length;

            _text.text = _targets[_currentTarget].ToString();
        }
    }
}