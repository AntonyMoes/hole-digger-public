using System.Collections;
using _Game.Scripts.Scheduling;
using DG.Tweening;
using GeneralUtils;
using UnityEngine;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.Audio {
    public class AudioPlayer {
        private const int AllowedStopFrames = 5;
        // private const float LoopCorrectionDuration = 0.05f;

        private readonly AudioSource _source;
        private readonly IScheduler _scheduler;

        private readonly Event _onStop = new Event();
        public IEvent OnStop => _onStop;

        public AudioType Type { get; private set; }

        private readonly UpdatedValue<float> _correctionVolume = new UpdatedValue<float>();
        public readonly UpdatedValue<float> Volume = new UpdatedValue<float>(setter: Mathf.Clamp01);
        private IUpdatedValue<float> _globalVolume;

        private Tween _correctionTween;
        private Coroutine _checkStopCoroutine;
        private int _stopFrames;
        private bool _stopping;

        public bool Mute {
            get => _source.mute;
            set => _source.mute = value;
        }

        public AudioPlayer(AudioSource source, IScheduler scheduler) {
            _source = source;
            _scheduler = scheduler;

            Volume.Subscribe(OnVolumeUpdate);
            _correctionVolume.Subscribe(OnVolumeUpdate);
        }

        public void Play(AudioClip clip, AudioType type, float volume, bool loop, IUpdatedValue<float> globalVolume) {
            _source.clip = clip;
            Type = type;
            _globalVolume = globalVolume;
            _globalVolume.Subscribe(OnVolumeUpdate, true);
            Volume.Value = volume;
            _correctionVolume.Value = 1f;
            _source.loop = loop;
            _source.Play();

            _scheduler.StartCoroutine(CheckStop(), coro => _checkStopCoroutine = coro);
            // if (loop) {
            //     _correctionTween = DOTween.Sequence()
            //         .Append(DOVirtual.Float(0f, 1f, LoopCorrectionDuration, value => _correctionVolume.Value = value))
            //         .AppendInterval(clip.length - LoopCorrectionDuration * 2)
            //         .Append(DOVirtual.Float(_correctionVolume.Value, 0f, LoopCorrectionDuration,
            //             value => _correctionVolume.Value = value));
            // }
        }

        public void Pause(bool pause) {
            if (pause) {
                _source.Pause();
            } else {
                _source.UnPause();
            }
        }

        public void Stop(float time = 0f) {
            if (_stopping) {
                return;
            }

            _stopping = true;
            _correctionTween?.Kill();
            _correctionTween =
                DOVirtual.Float(_correctionVolume.Value, 0f, time, value => _correctionVolume.Value = value)
                    .OnComplete(() => {
                        _source.Stop();
                        PerformStop();
                    });
        }

        private void PerformStop() {
            _stopping = false;
            _correctionTween?.Kill();
            _correctionTween = null;
            _scheduler.StopCoroutine(_checkStopCoroutine);
            _checkStopCoroutine = null;
            _globalVolume.Unsubscribe(OnVolumeUpdate);
            _globalVolume = null;
            _source.DOKill();
            _onStop.Invoke();
        }

        private void OnVolumeUpdate(float _) {
            _source.volume = Volume.Value * _globalVolume?.Value ?? 0f * _correctionVolume.Value;
        }

        private IEnumerator CheckStop() {
            while (true) {
                if (_source.isPlaying) {
                    _stopFrames = 0;
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                if (_stopFrames >= AllowedStopFrames) {
                    break;
                }

                _stopFrames += 1;
                yield return new WaitForEndOfFrame();
            }

            _stopFrames = 0;

            PerformStop();
        }
    }
}