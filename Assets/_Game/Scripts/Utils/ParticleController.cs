using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Utils {
    public class ParticleController : MonoBehaviour {
        [SerializeField] private ParticleSystem _particleSystem;
        public ParticleSystem ParticleSystem => _particleSystem;

        private readonly UpdatedValue<bool> _isPlaying = new UpdatedValue<bool>();
        public IUpdatedValue<bool> IsPlaying => _isPlaying;

        public void Play() {
            _particleSystem.Play();
            UpdatePlaying();
        }

        private void UpdatePlaying() {
            _isPlaying.Value = _particleSystem.isPlaying;
        }

        private void Update() {
            UpdatePlaying();
        }
    }
}