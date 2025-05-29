using System;
using UnityEngine;

namespace _Game.Scripts.Game.Level.DynamicTerrain {
    public class DynamicMeshCellView : MonoBehaviour {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshCollider _meshCollider;

        public IDynamicMeshCell Cell { get; private set; }
        
        public IUVSettings UVSettings { get; set; }

        public void Init(IDynamicMeshCell cell, Func<DynamicVertex, Vector3> getPosition) {
            gameObject.SetActive(true);
            Cell = cell;

            var mesh = cell.GenerateMesh(getPosition, transform.localRotation, transform.localScale, UVSettings);
            _meshFilter.sharedMesh = mesh;
            _meshCollider.sharedMesh = mesh;
        }

        public void Clear() {
            Cell.Dispose();
        }
    }
}