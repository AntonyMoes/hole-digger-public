using System;
using _Game.Scripts.Audio;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.Data.Configs.Meta.ResourceValue;
using _Game.Scripts.Game.Resource;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging {
    public class Drop : MonoBehaviour {
        [SerializeField] private SoundConfig[] _collisionSounds;

        private readonly Rng _rng = new Rng();
        private Action<Drop> _onCollect;
        private Action<Drop> _onRemove;

        public IResourceValue DropValue { get; private set; }

        public void Init(ResourceValueConfig dropValue, Action<Drop> onCollect, Action<Drop> onRemove) {
            DropValue = dropValue.Value;
            _onCollect = onCollect;
            _onRemove = onRemove;
        }

        public void Collect() {
            _onCollect?.Invoke(this);
        }

        public void Remove() {
            _onRemove(this);
        }

        private void OnCollisionEnter(Collision collision) {
            // var clip = _rng.NextChoice(new[] { "Tink_0", "Tink_1", "Tink_2" });
            var sound = _rng.NextChoice(_collisionSounds);
            AudioController.Instance.Play(sound/*, 0.7f*/);
        }
    }
}