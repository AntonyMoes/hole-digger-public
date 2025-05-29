using UnityEngine;

namespace _Game.Scripts.Game.Level.DynamicTerrain {
    public interface IDynamicMeshView {
        public Vector3 CellSize { get; }
        public Vector3Int MeshSizeInCells { get; }
        // public Vector3Int GetCellIndex(Vector3Int relativePosition);
    }
}