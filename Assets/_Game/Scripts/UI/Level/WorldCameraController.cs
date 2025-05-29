using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Game.Scripts.UI.Level {
    public class WorldCameraController : IWorldCameraController, IWorldPointProvider {
        private readonly List<Camera> _cameras = new List<Camera>();

        public void RegisterCamera(Camera camera) {
            if (!_cameras.Contains(camera)) {
                _cameras.Add(camera);
            }
        }

        public void UnregisterCamera(Camera camera) {
            _cameras.Remove(camera);
        }

        public Vector3 WorldToScreenPoint(Vector3 worldPosition) {
            return _cameras.Last().WorldToScreenPoint(worldPosition);
        }
    }
}