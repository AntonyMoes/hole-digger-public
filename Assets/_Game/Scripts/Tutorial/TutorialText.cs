using TMPro;
using UnityEngine;

namespace _Game.Scripts.Tutorial {
    public class TutorialText : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _text;

        public void Init(string text) {
            _text.SetText(text);
        }
    }
}