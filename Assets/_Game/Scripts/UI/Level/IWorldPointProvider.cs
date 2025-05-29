using UnityEngine;

namespace _Game.Scripts.UI.Level {
    public interface IWorldPointProvider {
        public Vector3 WorldToScreenPoint(Vector3 worldPosition);
    }
}