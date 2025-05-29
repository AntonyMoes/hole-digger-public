using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Utils;
using UnityEngine;

namespace _Game.Scripts.Game.Level {
    public class LevelScroll : MonoBehaviour {
        [SerializeField] private Transform _scrollPositionOrigin;
        [SerializeField] private Transform _levelSegment;
        [SerializeField] private Transform _segmentParent;

        [Header("Settings")]
        [SerializeField] private float _segmentLength = 25f;
        [Range(1, 10)] [SerializeField] private int _segmentsActive = 2;

        private Pool<Transform> _segmentPool;
        private readonly List<SegmentData> _segments = new List<SegmentData>();

        private void Awake() {
            _segmentPool = new Pool<Transform>(() => {
                    var segment = Instantiate(_levelSegment, _segmentParent);
                    segment.gameObject.SetActive(false);
                    return segment;
                }, _segmentsActive,
                segment => segment.gameObject.SetActive(true),
                segment => segment.gameObject.SetActive(false));
        }

        public void UpdateScrollTargetPosition(Vector3 targetPosition) {
            var distanceVector = targetPosition - _scrollPositionOrigin.position;
            var position = Vector3.Dot(distanceVector, _scrollPositionOrigin.forward);
            var currentPosition = Mathf.Max(position, 0f);

            var currentIndex = Mathf.FloorToInt(currentPosition / _segmentLength);
            var segmentsToSpawn = Enumerable
                .Range(currentIndex, _segmentsActive)
                .ToHashSet();
            var segmentsToRelease = new List<SegmentData>();

            foreach (var segment in _segments) {
                if (segmentsToSpawn.Contains(segment.Index)) {
                    segmentsToSpawn.Remove(segment.Index);
                } else {
                    segmentsToRelease.Add(segment);
                }
            }

            foreach (var segment in segmentsToRelease) {
                _segments.Remove(segment);
                segment.Segment.Release();
            }

            foreach (var index in segmentsToSpawn) {
                var segment = _segmentPool.Get();
                segment.Object.localPosition = Vector3.forward * index * _segmentLength;
                _segments.Add(new SegmentData {
                    Index = index,
                    Segment = segment
                });
            }
        }

        private class SegmentData {
            public int Index;
            public Pool<Transform>.IHandler Segment;
        }
    }
}