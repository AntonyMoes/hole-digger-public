using UnityEngine;

namespace _Game.Scripts.Data.Configs {
    [CreateAssetMenu(menuName = Configs.MenuItem + nameof(SoundConfig), fileName = nameof(SoundConfig))]
    public class SoundConfig : Config {
        [SerializeField] private AudioClip _clip;
        public AudioClip Clip => _clip;

        [SerializeField] private Audio.AudioType _audioType;
        public Audio.AudioType AudioType => _audioType;

        [Range(0f, 1f)] [SerializeField] private float _volume = 0.5f;
        public float Volume => _volume;
    }
}