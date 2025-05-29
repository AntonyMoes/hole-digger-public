using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Game.Level {
    public class LevelEntranceLight : MonoBehaviour {
        [SerializeField] private Light _entranceLight;
        [SerializeField] private Transform _distanceOrigin;

        [Header("Settings")]
        [SerializeField] private float _coefficient;
        [SerializeField] private float _initialIntensity;

        private Vector3 _lastPosition;

        public void UpdateTargetPosition(Vector3 targetPosition) {
            _lastPosition = targetPosition;
            var distance = Vector3.Distance(targetPosition, _distanceOrigin.position);
            _entranceLight.intensity = Mathf.Max(0f, CalculateIntensity(distance));
        }

        private float CalculateIntensity(float distance) {
            return _initialIntensity - distance * _coefficient;
        }

#if UNITY_EDITOR
        public void UpdateTargetPositionEditor() {
            UpdateTargetPosition(_lastPosition);
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LevelEntranceLight))]
    public class LevelEntranceLightEditor : Editor {
        public override void OnInspectorGUI() {
            var view = (LevelEntranceLight) target;

            base.OnInspectorGUI();
            if (GUI.changed) {
                view.UpdateTargetPositionEditor();
            }
        }
    }
#endif
}