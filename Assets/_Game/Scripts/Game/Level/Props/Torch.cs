using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Props {
    public class Torch : MonoBehaviour {
        [SerializeField] private Light _light;
        [SerializeField] private ParticleSystem _particles;

        [Header("Settings")]
        [Range(0, 3)] [SerializeField] private float _enableAnimationLength;
        [Range(0, 3)] [SerializeField] private float _disableAnimationLength;

        private Tween _toggleTween;
        private float _intensity;

        private void Awake() {
            _intensity = _light.intensity;
        }

        public void Toggle(bool active, bool instant = false) {
            _toggleTween?.Complete(true);

            var main = _particles.main;
            if (!active) {
                main.loop = false;
                var duration = instant ? 0f : _disableAnimationLength;
                _toggleTween = DOTween.Sequence()
                    .Insert(0, _light.DOIntensity(0, duration))
                    .InsertCallback(duration, () => {
                        main.loop = true;
                        _particles.Stop();
                    })
                    .OnComplete(() => _toggleTween = null);
            } else {
                var duration = instant ? 0f : _enableAnimationLength;
                _particles.Play();
                _toggleTween = _light.DOIntensity(_intensity, duration)
                    .OnComplete(() => _toggleTween = null);
            }
        }
    }
}