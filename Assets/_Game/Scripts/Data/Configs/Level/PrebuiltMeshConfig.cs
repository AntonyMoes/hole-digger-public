using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Game.Level.DynamicTerrain;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Level {
    [CreateAssetMenu(menuName = Configs.LevelMenuItem + nameof(PrebuiltMeshConfig), fileName = nameof(PrebuiltMeshConfig))]
    public class PrebuiltMeshConfig : Config {
        [SerializeField] private Mesh _mesh;
        public Mesh Mesh => _mesh;

        [SerializeField] private PointDeformData[] _pointData;
        public PointDeformData[] PointData => _pointData;

        [Serializable]
        public struct PointDeformData {
            public int index;
            public Vector3Int fromPoint;
            public Vector3Int toPoint;
            public float distance;
            public Vector3 offset;

            public static PointDeformData Default = new PointDeformData();

            public bool IsDefault => index == default &&
                                     fromPoint == default &&
                                     toPoint == default &&
                                     distance == default &&
                                     offset == default;
        }

#if UNITY_EDITOR
        public void EditorSetPointData(PointDeformData[] pointData) {
            _pointData = pointData;
        }
#endif
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(PrebuiltMeshConfig))]
    public class PrebuiltMeshConfigEditor : Editor {
        private static readonly Vector3[] DeformableVertices = PrebuiltMeshCell.DeformableVertices
            .Select(vertex => (Vector3) vertex / 2)
            .ToArray();

        private const float MaxEdgeExtruding = 0.051f;
        private static readonly float MaxEdgeDistance = Mathf.Sqrt(2 * Mathf.Pow(MaxEdgeExtruding, 2));

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var config = (PrebuiltMeshConfig) target;

            if (EditorUtility.IsPersistent(config)) {
                if (GUILayout.Button("Calculate")) {
                    CalculateMeshData(config);
                    EditorUtility.SetDirty(config);
                }
            }
        }

        private void CalculateMeshData(PrebuiltMeshConfig config) {
            if (config.Mesh == null) {
                config.EditorSetPointData(Array.Empty<PrebuiltMeshConfig.PointDeformData>());
                return;
            }

            var fromToPairs = new Dictionary<(Vector3 from, Vector3 to),
                (Func<Vector3, Vector2> getEdgeCoordinates, Func<Vector3, float> getOtherCoordinate)>();
            for (var i = 0; i < DeformableVertices.Length; i++) {
                var vertex1 = DeformableVertices[i];
                for (var j = i + 1; j < DeformableVertices.Length; j++) {
                    var vertex2 = DeformableVertices[j];
                    if (VerticesHaveEdge(vertex1, vertex2, out var getEdgeCoordinates, out var getOtherCoordinate)
                        && !fromToPairs.ContainsKey((vertex2, vertex1))) {
                        fromToPairs.Add((vertex1, vertex2), (getEdgeCoordinates, getOtherCoordinate));
                    }
                }
            }

            var mesh = config.Mesh;

            var data = new List<PrebuiltMeshConfig.PointDeformData>();
            var verticesWithoutAdjacentEdgeVertex = new List<int>();
            for (var i = 0; i < mesh.vertexCount; i++) {
                var vertex = mesh.vertices[i];

                foreach (var ((from, to), (getEdgeCoordinates, getOtherCoordinate)) in fromToPairs) {
                    var edgeCoordinates = getEdgeCoordinates(from);
                    var edgeDistance = Vector2.Distance(edgeCoordinates, getEdgeCoordinates(vertex));
                    if (edgeDistance > MaxEdgeDistance) {
                        continue;
                    }

                    Vector3 distanceVertex;
                    Vector3 offset;
                    if (edgeDistance == 0) {
                        distanceVertex = vertex;
                        offset = Vector3.zero;
                    } else if (!FindVertexOffset(i, getEdgeCoordinates, edgeCoordinates, mesh, out distanceVertex,
                                   out offset)) {
                        verticesWithoutAdjacentEdgeVertex.Add(i);
                        continue;
                    }

                    var min = getOtherCoordinate(from);
                    var max = getOtherCoordinate(to);
                    var value = getOtherCoordinate(distanceVertex);
                    var distance = (value - min) / (max - min);

                    data.Add(new PrebuiltMeshConfig.PointDeformData {
                        index = i,
                        fromPoint = Vector3Int.RoundToInt(from * IDynamicMeshCell.RelativeSize),
                        toPoint = Vector3Int.RoundToInt(to * IDynamicMeshCell.RelativeSize),
                        distance = distance,
                        offset = offset
                    });

                    break;
                }
            }

            foreach (var index in verticesWithoutAdjacentEdgeVertex) {
                var vertexData = data.FirstOrDefault(d => mesh.vertices[d.index] == mesh.vertices[index]);
                if (vertexData.IsDefault) {
                    continue;
                }

                vertexData.index = index;
                data.Add(vertexData);
            }

            config.EditorSetPointData(data.ToArray());
        }

        private static bool VerticesHaveEdge(Vector3 vertex1, Vector3 vertex2,
            out Func<Vector3, Vector2> getEdgeCoordinates,
            out Func<Vector3, float> getOtherCoordinate) {
            switch (vertex1.x == vertex2.x, vertex1.y == vertex2.y, vertex1.z == vertex2.z) {
                case (true, true, _):
                    getEdgeCoordinates = vertex => new Vector2(vertex.x, vertex.y);
                    getOtherCoordinate = vertex => vertex.z;
                    return true;
                case (true, _, true):
                    getEdgeCoordinates = vertex => new Vector2(vertex.x, vertex.z);
                    getOtherCoordinate = vertex => vertex.y;
                    return true;
                case (_, true, true):
                    getEdgeCoordinates = vertex => new Vector2(vertex.y, vertex.z);
                    getOtherCoordinate = vertex => vertex.x;
                    return true;
                default:
                    getEdgeCoordinates = null;
                    getOtherCoordinate = null;
                    return false;
            }
        }

        private static bool FindVertexOffset(int index, Func<Vector3, Vector2> getEdgeCoordinates,
            Vector2 targetEdgeCoordinates, Mesh mesh, out Vector3 edgeVertex, out Vector3 offset) {
            var defaultValue = Vector3.zero;
            var vertex = Position(index);
            var closestEdgeVertex = Enumerable.Range(0, mesh.triangles.Length)
                // get all vertices that share a triangle with the target
                .Where(i => mesh.triangles[i] == index)
                .Select(i => i - (i % 3))
                .SelectMany(i => mesh.triangles.Skip(i).Take(3)
                    .Where(j => j != i)
                    // and are on the target edge
                    .Where(j => getEdgeCoordinates(Position(j)) == targetEdgeCoordinates))
                // find the closest
                .Select(Position)
                .Aggregate(defaultValue, (closest, next) =>
                    Vector3.Distance(next, vertex) < Vector3.Distance(closest, vertex)
                        ? next
                        : closest);

            if (closestEdgeVertex == defaultValue) {
                edgeVertex = default;
                offset = default;
                return false;
            }

            edgeVertex = closestEdgeVertex;
            offset = vertex - closestEdgeVertex;
            return true;

            Vector3 Position(int idx) => mesh.vertices[idx];
        }
    }
#endif
}