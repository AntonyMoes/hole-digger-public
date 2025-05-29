using DG.Tweening;
using GeneralUtils;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Game.Level {
    public class LevelCamera : MonoBehaviour {
        [SerializeField] private Camera _camera;
        public Camera Camera => _camera;

        [Header("Settings")]
        [Range(1, 30)] [SerializeField] private float _distanceFromDigZone = 10f;
        [Range(0.1f, 3f)] [SerializeField] private float _cameraMoveTime = 1f;
        [Range(20f, 90f)] [SerializeField] private float _horizontalFov = 43f;

        private Tween _cameraTween;
        private Vector3 _lastBoundary;

        private readonly UpdatedValue<Vector3> _position = new UpdatedValue<Vector3>();
        public IUpdatedValue<Vector3> Position => _position;

        private readonly UpdatedValue<float> _movement = new UpdatedValue<float>();
        public IUpdatedValue<float> Movement => _movement;

        private void Awake() {
            SetFov();
        }

        public void MoveCamera(Vector3 zoneBoundary, bool instant = false) {
            _lastBoundary = zoneBoundary;

            var targetPosition = zoneBoundary - transform.forward * _distanceFromDigZone;
            var wasMoving = _cameraTween != null;
            _cameraTween?.Kill();
            _cameraTween = null;
            _movement.Value = 0;

            if (instant) {
                transform.position = targetPosition;
                Update();
                _movement.Value = 1;
                return;
            }

            var ease = wasMoving ? Ease.OutSine : Ease.InOutSine;
            _cameraTween = DOTween.Sequence()
                .Insert(0, transform
                    .DOMove(targetPosition, _cameraMoveTime)
                    .SetEase(ease))
                .Insert(0, DOVirtual
                    .Float(0, 1, _cameraMoveTime, value => _movement.Value = value)
                    .SetEase(ease))
                .OnComplete(() => {
                    _cameraTween = null;
                    _movement.Value = 1;
                });
        }

        private void SetFov() {
            _camera.fieldOfView = Mathf.Atan(Mathf.Tan(_horizontalFov / 2 / Mathf.Rad2Deg) / _camera.aspect) *
                                  Mathf.Rad2Deg * 2;
        }

        private void Update() {
            _position.Value = transform.position;
            SetFov();
        }

#if UNITY_EDITOR
        public void UpdateSettings() {
            _cameraTween?.Kill();
            MoveCamera(_lastBoundary, true);
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LevelCamera))]
    public class LevelCameraEditor : Editor {
        public override void OnInspectorGUI() {
            var controller = (LevelCamera) target;

            base.OnInspectorGUI();
            if (GUI.changed) {
                controller.UpdateSettings();
            }
        }
    }
#endif
}