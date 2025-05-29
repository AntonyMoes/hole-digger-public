using UnityEngine;

namespace _Game.Scripts.Game.Level.DynamicTerrain {
    public interface IUVSettings {
        public (Vector2 origin, Vector2 right, Vector2 up, Vector2 sideSize)[] UVData { get; }
    }
}