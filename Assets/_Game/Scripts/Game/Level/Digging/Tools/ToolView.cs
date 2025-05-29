using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Audio;
using _Game.Scripts.Data.Configs;
using DG.Tweening;
using GeneralUtils;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging.Tools {
    public abstract class ToolView : MonoBehaviour, IToolView {
        [SerializeField] private Transform _toolObjectContainer;

        private readonly Rng _rng = new Rng();

        private ITool _tool;
        private GameObject _toolObject;
        private Tween _animationTween;
        private float _lastHeightRatio;
        private Func<GameObject> _prefabProvider;
        private IDisposable _prefabUpdateSubscription;
        private IReadOnlyList<SoundConfig> _sounds;

        public void Init(ITool tool, IEvent prefabUpdateEvent, Func<GameObject> prefabProvider, IEnumerable<SoundConfig> sounds) {
            _tool = tool;
            _sounds = sounds.ToArray();
            _prefabProvider = prefabProvider;
            _prefabUpdateSubscription = prefabUpdateEvent.Subscribe(UpdatePrefab);
            UpdatePrefab();
        }

        private void UpdatePrefab() {
            if (_toolObject != null) {
                Destroy(_toolObject);
            }

            _toolObject = Instantiate(_prefabProvider(), _toolObjectContainer);
        }

        public void Play(Vector3 cellPosition, Vector3 impactPoint, float heightRatio) {
            _lastHeightRatio = heightRatio;
            _animationTween?.Kill();
            
            var sound = _rng.NextChoice(_sounds);
            AudioController.Instance.Play(sound);

            gameObject.SetActive(true);
            transform.position = GetPosition(cellPosition, impactPoint);

            _animationTween = PerformPlay(heightRatio)
                .OnComplete(() => {
                    gameObject.SetActive(false);
                    _animationTween = null;
                });
        }

        protected abstract Vector3 GetPosition(Vector3 cellPosition, Vector3 impactPoint);

        protected abstract Tween PerformPlay(float heightRatio);

        public void Stop() {
            _animationTween?.Complete(true);
        }

#if UNITY_EDITOR
        public void PlayEditor() {
            Play(transform.position, transform.position, _lastHeightRatio);
        }
#endif

        public void Dispose() {
            _prefabUpdateSubscription?.Dispose();
            _animationTween?.Kill();
            Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ToolView), true)]
    public class ToolControllerEditor : Editor {
        public override void OnInspectorGUI() {
            var controller = (ToolView) target;

            base.OnInspectorGUI();
            if (GUILayout.Button("Play")) {
                controller.PlayEditor();
            }
        }
    }
#endif
}