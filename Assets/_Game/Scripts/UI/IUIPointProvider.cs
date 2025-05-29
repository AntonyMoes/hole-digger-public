using UnityEngine;

namespace _Game.Scripts.UI {
    public interface IUIPointProvider {
        public bool ScreenPointToWorldPointInRectangle(RectTransform rect, Vector2 screenPosition, out Vector3 position);
    }
}