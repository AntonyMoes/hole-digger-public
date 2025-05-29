using _Game.Scripts.Game.Level.DynamicTerrain;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging {
    public class CellView : MonoBehaviour {
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private DynamicMeshCellView _cellView;
        public DynamicMeshCellView View => _cellView;


        [Header("Settings")]
        [SerializeField] private Color _backColor;
        [SerializeField] private Color _frontColor;

        private IUpdatedValue<float> _cameraMovement;
        private float _colorInterpolation = -1;

        private void Awake() {
            SetColorInterpolation(0);
        }

        public void SubscribeToCameraMovement(IUpdatedValue<float> cameraMovement) {
            _cameraMovement = cameraMovement;
            _cameraMovement.Subscribe(SetColorInterpolation, true);
        }

        private void SetColorInterpolation(float value) {
            var clamped = Mathf.Clamp01(value);
            if (Mathf.Approximately(_colorInterpolation, clamped)) {
                return;
            }

            _colorInterpolation = clamped;
            _renderer.material.color = Color.Lerp(_backColor, _frontColor, _colorInterpolation);
        }

        private void OnDestroy() {
            _cameraMovement?.Unsubscribe(SetColorInterpolation);
        }
    }
}