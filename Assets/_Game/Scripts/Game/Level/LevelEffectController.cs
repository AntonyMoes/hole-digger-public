using _Game.Scripts.Utils;
using UnityEngine;

namespace _Game.Scripts.Game.Level {
    public class LevelEffectController : MonoBehaviour {
        [SerializeField] private ParticleController _poofPrefab;
        [SerializeField] private Transform _effectParent;

        private Pool<ParticleController> _poofPool;

        private void Awake() {
            _poofPool = new Pool<ParticleController>(() => Instantiate(_poofPrefab, _effectParent));
        }

        public void PlayPoof(Vector3 position, float size = 1f) {
            var poof = _poofPool.Get();

            // var size = Mathf.Sqrt(_digZone.CellSize.x * _digZone.CellSize.y) / 2.5f;
            var actualSize = size / 2.5f;
            var main = poof.Object.ParticleSystem.main;
            main.startSize = new ParticleSystem.MinMaxCurve(actualSize / 2, actualSize);
            poof.Object.transform.position = position;

            poof.Object.Play();
            poof.Object.IsPlaying.WaitFor(false, poof.Release);
        }
    }
}