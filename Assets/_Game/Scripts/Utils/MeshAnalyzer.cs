using UnityEngine;

namespace _Game.Scripts.Utils {
    [RequireComponent(typeof(MeshFilter))]
    public class MeshAnalyzer : MonoBehaviour {
        [SerializeField] private bool _enabled;

        private MeshFilter _meshFilter;

        private void OnDrawGizmos() {
            if (!_enabled) {
                return;
            }

            if (_meshFilter == null) {
                _meshFilter = GetComponent<MeshFilter>();
            }

            var oldColor = Gizmos.color;

            var mesh = _meshFilter.sharedMesh;
            for (var i = 0; i < mesh.vertexCount; i++) {
                Gizmos.color = mesh.uv.Length > 0 ? new Color(mesh.uv[i].x, 0, mesh.uv[i].y) : Color.black;
                var vertex = mesh.vertices[i] + _meshFilter.transform.position;
                var normal = mesh.normals[i];
                Gizmos.DrawLine(vertex, vertex + normal * 0.1f);
            }

            Gizmos.color = oldColor;
        }
    }
}