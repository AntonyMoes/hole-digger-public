using _Game.Scripts.Utils;
using DG.Tweening;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging.Tools {
    public class ShovelView : ToolView {
        [SerializeField] private Transform _initialRotation;
        [SerializeField] private Transform _move;
        [SerializeField] private Transform _shovel;

        [Header("Settings")]
        [Range(0.05f, 0.5f)] [SerializeField] private float _digSwingDuration;
        [Range(0f, 180f)] [SerializeField] private float _digSwingAngle;
        [Range(0f, 180f)] [SerializeField] private float _topDigSwingAngle;
        [SerializeField] private Ease _digSwingEase;

        [Range(0.05f, 0.5f)] [SerializeField] private float _digDuration;
        [Range(0f, 3f)] [SerializeField] private float _digDistance;
        [SerializeField] private Ease _digEase;
        [SerializeField] private Ease _digBackEase;

        [Range(0f, 30f)] [SerializeField] private float _maxInitialAngle;
        [Range(0f, 60f)] [SerializeField] private float _maxHeightAngle;

        private readonly Rng _rng = new Rng();

        protected override Vector3 GetPosition(Vector3 cellPosition, Vector3 impactPoint) {
            return cellPosition;
        }

        protected override Tween PerformPlay(float heightRatio) {
            var initialRotation = _rng.NextVector3(-Vector3.one, Vector3.one) * _maxInitialAngle +
                                  Vector3.left * (_maxHeightAngle * heightRatio);
            _initialRotation.localRotation = Quaternion.Euler(initialRotation);
            _shovel.localRotation = Quaternion.identity;
            _move.localPosition = Vector3.down * _digDistance;

            var swingAngle = heightRatio >= 1f ? _topDigSwingAngle : _digSwingAngle;
            return DOTween.Sequence()
                .Append(_move
                    .DOLocalMoveY(0, _digDuration)
                    .SetEase(_digEase))
                .Append(_shovel
                    .DOLocalRotate(Vector3.right * swingAngle, _digSwingDuration)
                    .SetEase(_digSwingEase))
                .Insert(_digDuration, _move
                    .DOLocalMoveY(-_digDistance, _digSwingDuration)
                    .SetEase(_digBackEase));
        }
    }
}