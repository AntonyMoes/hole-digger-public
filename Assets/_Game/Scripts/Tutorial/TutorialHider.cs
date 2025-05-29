using GeneralUtils;
using UnityEngine;
using UnityEngine.UI;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.Tutorial {
    public class TutorialHider : MonoBehaviour {
        [SerializeField] private Image _hole;
        
        private readonly Event _clickEvent = new Event();
        public IEvent ClickEvent => _clickEvent;

        public void Init(bool canClickThroughHole) {
            _hole.raycastTarget = !canClickThroughHole;
        }

        public void OnClick() {
            _clickEvent.Invoke();
        }
    }
}