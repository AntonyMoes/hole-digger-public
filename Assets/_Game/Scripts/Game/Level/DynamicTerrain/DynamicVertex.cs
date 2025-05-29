using UnityEngine;

namespace _Game.Scripts.Game.Level.DynamicTerrain {
    public class DynamicVertex {
        public bool CanBeOffset { get; private set; } = true;
        public Vector3Int RelativePosition;

        public void AddRelatedCell(bool canOffsetVertex) {
            CanBeOffset = CanBeOffset && canOffsetVertex;
        }
    }
}