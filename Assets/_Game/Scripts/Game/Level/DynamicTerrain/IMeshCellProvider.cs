using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.Game.Level.DynamicTerrain {
    public interface IMeshCellProvider {
        [CanBeNull] public IDynamicMeshCell GetCell(Vector3Int relativePosition, Vector3Int index);
    }
}