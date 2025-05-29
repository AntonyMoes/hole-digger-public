using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Scripts.Debug {
    public class DragComponent : MonoBehaviour {
        private RectTransform _rectTransform;

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();

            var eventTrigger = TryGetComponent<EventTrigger>(out var trigger)
                ? trigger
                : gameObject.AddComponent<EventTrigger>();

            var entry = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
            entry.callback.AddListener(OnDrag);
            eventTrigger.triggers.Add(entry);
        }

        private void OnDrag(BaseEventData eventData) {
            if (!(eventData is PointerEventData pointerData)) {
                return;
            }

            var currentPosition = _rectTransform.anchoredPosition;
            currentPosition.x += pointerData.delta.x;
            currentPosition.y += pointerData.delta.y;
            _rectTransform.anchoredPosition = currentPosition;
        }
    }
}