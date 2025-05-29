using System;
using UnityEngine;

namespace _Game.Scripts.Audio {
    public class AudioAdapter : MonoBehaviour, IDisposable {
        [SerializeField] private Transform _sourceParent;
        public Transform SourceParent => _sourceParent;

        [SerializeField] private AudioSource _sourcePrefab;
        public AudioSource SourcePrefab => _sourcePrefab;

        public void Dispose() {
            for (var i = 0; i < _sourceParent.childCount; i++) {
                var child = _sourceParent.GetChild(i).gameObject;
                Destroy(child);
            }
        }
    }
}