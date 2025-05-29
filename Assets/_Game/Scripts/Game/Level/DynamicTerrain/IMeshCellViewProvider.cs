using UnityEngine;

namespace _Game.Scripts.Game.Level.DynamicTerrain {
    public interface IMeshCellViewProvider {
        public DynamicMeshCellView InstantiateView(Vector3Int relativePosition, Vector3Int index, Transform parent);
    }
}