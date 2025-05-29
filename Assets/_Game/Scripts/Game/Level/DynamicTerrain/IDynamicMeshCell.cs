using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Game.Level.DynamicTerrain {
    public interface IDynamicMeshCell : IDisposable {
        public const int RelativeSize = 2;
        public Vector3Int RelativePosition { get; }
        public Vector3Int Index { get; }

        public void RegisterVertices(Dictionary<Vector3Int, DynamicVertex> pointDict);

        public Mesh GenerateMesh(Func<DynamicVertex, Vector3> getPosition, Quaternion rotation, Vector3 scale,
            IUVSettings uvSettings = null);
    }
}