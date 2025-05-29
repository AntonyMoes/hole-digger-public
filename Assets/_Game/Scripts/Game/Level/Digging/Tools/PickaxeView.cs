using _Game.Scripts.Utils;
using DG.Tweening;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging.Tools {
    public class PickaxeView : ToolView {
        [SerializeField] private Transform _initialRotation;
        [SerializeField] private Transform _move;
        [SerializeField] private Transform _pickaxe;

        [Header("Settings")]
        [Range(0.05f, 0.5f)] [SerializeField] private float _digSwingDuration;
        [Range(0f, 180f)] [SerializeField] private float _digSwingAngle;
        [SerializeField] private Ease _digSwingEase;
        [Range(0f, 2f)] [SerializeField] private float _digDistance;
        [SerializeField] private Ease _digEase;
        [Range(0.05f, 0.5f)] [SerializeField] private float _digSwingBackDuration;
        [Range(0f, 180f)] [SerializeField] private float _digSwingBackAngle;
        [SerializeField] private Ease _digSwingBackEase;

        [Range(0f, 30f)] [SerializeField] private float _maxInitialAngle;
        [Range(0f, 60f)] [SerializeField] private float _maxHeightAngle;
        [Range(-60f, 0f)] [SerializeField] private float _minHeightAngle;

        private readonly Rng _rng = new Rng();

        protected override Vector3 GetPosition(Vector3 cellPosition, Vector3 impactPoint) {
            return impactPoint;
        }

        protected override Tween PerformPlay(float heightRatio) {
            var initialRotation = _rng.NextVector3(-Vector3.one, Vector3.one) * _maxInitialAngle +
                                  Vector3.left * (_minHeightAngle + (_maxHeightAngle - _minHeightAngle) * heightRatio);
            _initialRotation.localRotation = Quaternion.Euler(initialRotation);
            _pickaxe.localRotation = Quaternion.Euler(Vector3.forward * _digSwingAngle);
            _move.localPosition = Vector3.back * _digDistance;

            return DOTween.Sequence()
                .Insert(0, _move
                    .DOLocalMoveZ(0, _digSwingDuration)
                    .SetEase(_digEase))
                .Insert(0, _pickaxe
                    .DOLocalRotate(Vector3.zero, _digSwingDuration)
                    .SetEase(_digSwingEase))
                .Insert(_digSwingDuration, _pickaxe
                    .DOLocalRotate(Vector3.forward * _digSwingBackAngle, _digSwingBackDuration)
                    .SetEase(_digSwingBackEase));
        }
    }
}