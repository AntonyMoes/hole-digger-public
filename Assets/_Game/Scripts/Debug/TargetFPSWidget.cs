using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.Debug {
    public class TargetFPSWidget : MonoBehaviour {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;

        [Header("Settings")]
        [SerializeField] private int[] _targets;

        private int _currentTarget = -1;

        private void Awake() {
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick() {
            _currentTarget += 1;
            if (_currentTarget == _targets.Length) {
                _currentTarget = -1;
            }

            if (_currentTarget == -1) {
                Application.targetFrameRate = -1;
                _text.text = "no target";
                return;
            }

            Application.targetFrameRate = _targets[_currentTarget];
            _text.text = _targets[_currentTarget].ToString();
        }
    }
}