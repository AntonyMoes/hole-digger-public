using UnityEngine;

namespace _Game.Scripts.UI {
    public class UIControllerView : MonoBehaviour {
        [SerializeField] private Transform _screenLayer;
        public Transform ScreenLayer => _screenLayer;

        [SerializeField] private Transform _windowLayer;
        public Transform WindowLayer => _windowLayer;

        [SerializeField] private Transform _loadingLayer;
        public Transform LoadingLayer => _loadingLayer;

        [SerializeField] private Transform _tutorialLevel;
        public Transform TutorialLevel => _tutorialLevel;

        [SerializeField] private Camera _uiCamera;
        public Camera UICamera => _uiCamera;

        public void Clear() {
            var layers = new[] {
                ScreenLayer,
                WindowLayer,
                TutorialLevel,
                LoadingLayer
            };

            foreach (var layer in layers) {
                for (var i = 0; i < layer.childCount; i++) {
                    Destroy(layer.GetChild(i).gameObject);
                }
            }
        }
    }
}