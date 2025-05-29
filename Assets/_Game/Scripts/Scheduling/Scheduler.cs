using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GeneralUtils;
using UnityEngine;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.Scheduling {
    public class Scheduler : MonoBehaviour, IScheduler {
        private static readonly Thread MainThread = Thread.CurrentThread;
        private readonly List<(IEnumerator, Action<Coroutine>)> _enumeratorsToStart = new List<(IEnumerator, Action<Coroutine>)>();
        private bool _quit;

        private readonly Event<float> _frameEvent = new Event<float>();
        public IEvent<float> FrameEvent => _frameEvent;

        private readonly Event<bool> _pauseEvent = new Event<bool>();
        public IEvent<bool> PauseEvent => _pauseEvent;

        private readonly Event _quitEvent = new Event();
        public IEvent QuitEvent => _quitEvent;
        
        private void Awake() {
            Application.quitting += OnApplicationQuit;
        }

        public void RunInMainThread(Action action) {
            StartCoroutine(Wrapper());

            IEnumerator Wrapper() {
                yield return new WaitForEndOfFrame();
                action?.Invoke();
            }
        }

        public void StartCoroutine(IEnumerator enumerator, Action<Coroutine> onStarted = null) {
            if (Thread.CurrentThread == MainThread) {
                var coroutine = base.StartCoroutine(enumerator);
                onStarted?.Invoke(coroutine);
            } else {
                _enumeratorsToStart.Add((enumerator, onStarted));
            }
        }


        public IDisposable SubscribeToPeriodicEvent(Action subscriber, float period) {
            var provider = new PeriodicEventProvider(period);
            FrameEvent.Subscribe(provider.OnFrame);
            provider.PeriodicEvent.Subscribe(subscriber);

            return new DisposeCallback(() => {
                provider.PeriodicEvent.Unsubscribe(subscriber);
                FrameEvent.Unsubscribe(provider.OnFrame);
            });
        }

        private void Update() {
            var deltaTime = UnityEngine.Time.deltaTime;
            _frameEvent.Invoke(deltaTime);

            if (_enumeratorsToStart.Count == 0) {
                return;
            }

            var enumerators = _enumeratorsToStart.ToArray();
            _enumeratorsToStart.Clear();
            foreach (var (enumerator, onStarted) in enumerators) {
                var coroutine = base.StartCoroutine(enumerator);
                onStarted?.Invoke(coroutine);
            }
        }

        private void OnApplicationPause(bool paused) {
            _pauseEvent.Invoke(paused);
        }

        private void OnApplicationQuit() {
            if (_quit) {
                return;
            }

            _quit = true;
            _quitEvent.Invoke();
        }

        private class PeriodicEventProvider {
            private readonly float _period;
            private float _remaining;

            private readonly Event _periodicEvent = new Event();
            public IEvent PeriodicEvent => _periodicEvent;


            public PeriodicEventProvider(float period) {
                _period = period;
                _remaining = period;
            }

            public void OnFrame(float deltaTime) {
                _remaining -= deltaTime;

                if (_remaining <= 0) {
                    _remaining += _period;
                    _periodicEvent.Invoke();
                }
            }
        }

        public void Dispose() {
            _enumeratorsToStart.Clear();
            _frameEvent.ClearSubscribers();
            _pauseEvent.ClearSubscribers();
            _quitEvent.ClearSubscribers();
        }
    }
}