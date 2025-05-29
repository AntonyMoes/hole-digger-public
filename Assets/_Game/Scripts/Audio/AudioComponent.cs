using _Game.Scripts.Data.Configs;
using UnityEngine;

namespace _Game.Scripts.Audio {
    public class AudioComponent : MonoBehaviour {
        public void Play(SoundConfig config) {
            AudioController.Instance.Play(config);
        }
    }
}