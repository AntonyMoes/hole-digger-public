using UnityEngine;

namespace _Game.Scripts.UI.Level {
    public interface IWorldCameraController {
        public void RegisterCamera(Camera camera);
        public void UnregisterCamera(Camera camera);
    }
}