using System;
using System.Collections.Generic;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.DI;
using _Game.Scripts.Scheduling;
using _Game.Scripts.Utils;
using GeneralUtils;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace _Game.Scripts.Audio {
    public class AudioController : Singleton<AudioController> {
        private readonly AudioAdapter _audioAdapter;
        private readonly Pool<AudioPlayer> _pool;
        private readonly HashSet<AudioPlayer> _usedPlayers = new HashSet<AudioPlayer>();

        private readonly UpdatedValue<float> _globalSoundVolume = new UpdatedValue<float>(1);
        private readonly UpdatedValue<float> _globalMusicVolume = new UpdatedValue<float>(1);

        [Inject]
        public AudioController(IScheduler scheduler, AudioAdapter audioAdapter /*, Settings controllerSettings*/) {
            _audioAdapter = audioAdapter;
            _pool = new Pool<AudioPlayer>(CreateSource, 3, onRelease: OnRelease);

            // controllerSettings.EnableSound.Subscribe(OnEnableSound, true);
            // controllerSettings.EnableMusic.Subscribe(OnEnableMusic, true);

            AudioPlayer CreateSource() {
                var source = Object.Instantiate(audioAdapter.SourcePrefab, audioAdapter.SourceParent);
                return new AudioPlayer(source, scheduler);
            }
        }

        private void OnRelease(AudioPlayer player) {
            _usedPlayers.Remove(player);
        }

        private void OnEnableSound(bool enabled) {
            _globalSoundVolume.Value = enabled ? 1f : 0f;
        }

        private void OnEnableMusic(bool enabled) {
            _globalMusicVolume.Value = enabled ? 1f : 0f;
        }

        public IAudioHandler Play([CanBeNull] SoundConfig config, bool loop = false) {
            if (config == null) {
                return null;
            }

            var handler = _pool.Get();
            _usedPlayers.Add(handler.Object);
            handler.Object.OnStop.SubscribeOnce(() => handler.Release());

            var globalVolume = config.AudioType switch {
                AudioType.Sound => _globalSoundVolume,
                AudioType.Music => _globalMusicVolume,
                _ => throw new ArgumentOutOfRangeException(nameof(config.AudioType), config.AudioType, null)
            };

            handler.Object.Play(config.Clip, config.AudioType, config.Volume, loop, globalVolume);
            return new PlayerHandler(handler.Object);
        }

        protected override void PerformDispose() {
            _audioAdapter.Dispose();
        }

        private class PlayerHandler : IAudioHandler {
            private AudioPlayer _player;

            public float Volume {
                get => _player?.Volume.Value ?? 1f;
                set {
                    if (_player != null) {
                        _player.Volume.Value = value;
                    }
                }
            }

            public bool Mute {
                get => _player?.Mute ?? false;
                set {
                    if (_player != null) {
                        _player.Mute = value;
                    }
                }
            }

            public PlayerHandler(AudioPlayer player) {
                _player = player;
                _player.OnStop.SubscribeOnce(() => _player = null);
            }

            public void Pause(bool pause) {
                _player?.Pause(pause);
            }

            public void Stop(float time = 0f) {
                _player?.Stop(time);
            }
        }

        /*public class Settings {
            public IUpdatedValue<bool> EnableSound;
            public IUpdatedValue<bool> EnableMusic;
        }*/
    }
}