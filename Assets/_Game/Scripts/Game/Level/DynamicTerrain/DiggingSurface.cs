using System;
using System.Collections.Generic;
using System.Linq;
using GeneralUtils;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Game.Level.DynamicTerrain {
    [Obsolete]
    public class DiggingSurface : MonoBehaviour {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshCollider _meshCollider;

        [Header("Settings")]
        [Range(0, 20)] [SerializeField] private float _width;
        [Range(0, 20)] [SerializeField] private float _height;
        [Range(0, 5)] [SerializeField] private float _depth;
        [Range(1, 16)] [SerializeField] private int _widthCells;
        [Range(1, 16)] [SerializeField] private int _heightCells;
        [Tooltip("0 - no offset, 1 - half cell size")]
        [Range(0, 1)] [SerializeField] private float _offset;
        [SerializeField] private bool _subdivideCells;
        [SerializeField] private bool _discreteShadows;

        private readonly Rng _rng = new Rng();
        private readonly List<Vector3> _relativeOffsets = new List<Vector3>();

        public void Init() {
            UpdateMesh();
        }

#if UNITY_EDITOR
        public void GenerateMeshEditor(bool regenerateOffsets) {
            if (regenerateOffsets) {
                _relativeOffsets.Clear();
            }

            UpdateMesh();
        }
#endif

        private void UpdateMesh() {
            var mesh = GenerateMesh();
            _meshFilter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
        }

        private Mesh GenerateMesh() {
            var cellSize = new Vector3(_width / _widthCells, _height / _heightCells, _depth);
            var (vertices, triangles) = GenerateMeshData();

            for (var i = 0; i < vertices.Count; i++) {
                if (_relativeOffsets.Count <= i) {
                    _relativeOffsets.Add(GetOffset(_rng));
                }

                vertices[i].SetPosition(cellSize, _relativeOffsets[i] * _offset);
            }

            var mesh = new Mesh();
            if (!_discreteShadows) {
                mesh.vertices = vertices.Select(v => v.Position).ToArray();
                mesh.normals = vertices.Select(v => v.CalculateNormal()).ToArray();
                mesh.triangles = triangles
                    .SelectMany(tri => Enumerable.Empty<int>()
                        .Append(tri.A.Index)
                        .Append(tri.B.Index)
                        .Append(tri.C.Index))
                    .ToArray();
            } else {
                mesh.vertices = triangles
                    .SelectMany(tri => Enumerable.Empty<Vector3>()
                        .Append(tri.A.Position)
                        .Append(tri.B.Position)
                        .Append(tri.C.Position))
                    .ToArray();
                mesh.normals = triangles
                    .SelectMany(tri => Enumerable.Repeat(tri.CalculateNormal(), 3))
                    .ToArray();
                mesh.triangles = Enumerable.Range(0, mesh.vertices.Length).ToArray();
            }

            return mesh;
        }

        private static Vector3 GetOffset(Rng rng) {
            return new Vector3(
                rng.NextFloat(-1, 1),
                rng.NextFloat(-1, 1),
                rng.NextFloat(-1, 1)
            );
        }

        private (List<Vertex>, List<Triangle>) GenerateMeshData() {
            var horizontalCount = _subdivideCells
                ? _widthCells * 2 + 1
                : _widthCells + 1;
            var verticalCount = _heightCells + 1;

            var startRelativePosition = -new Vector2(_widthCells, _heightCells) / 2;
            var relativeIncrement = new Vector2(_subdivideCells ? 0.5f : 1f, 1f);
            var verticalSubdivisionIncrement = 0.5f;

            var vertices = new List<Vertex>();
            var triangles = new List<Triangle>();

            for (var i = 0; i < horizontalCount; i++) {
                var subdivisionColumn = _subdivideCells && i % 2 != 0;
                for (var j = 0; j < verticalCount; j++) {
                    if (subdivisionColumn && j == verticalCount - 1) {
                        continue;
                    }

                    // generate vertex
                    var vertex = new Vertex(vertices);
                    var horizontalPosition = startRelativePosition.x + relativeIncrement.x * i;
                    var verticalPosition = startRelativePosition.y + relativeIncrement.y * j + (subdivisionColumn
                        ? verticalSubdivisionIncrement
                        : 0f);
                    vertex.RelativePosition = new Vector2(horizontalPosition, verticalPosition);

                    if (i == 0) {
                        continue;
                    }

                    // generate triangles
                    if (!_subdivideCells) {
                        if (j > 0) {
                            var b = GetVertexByLoopIndex(i, j - 1);
                            var c = GetVertexByLoopIndex(i - 1, j);
                            triangles.Add(new Triangle(vertex, b, c));
                        }

                        if (j < verticalCount - 1) {
                            var b = GetVertexByLoopIndex(i - 1, j);
                            var c = GetVertexByLoopIndex(i - 1, j + 1);
                            triangles.Add(new Triangle(vertex, b, c));
                        }
                    } else {
                        if (subdivisionColumn) {
                            var b = GetVertexByLoopIndex(i - 1, j);
                            var c = GetVertexByLoopIndex(i - 1, j + 1);
                            triangles.Add(new Triangle(vertex, b, c));
                        } else {
                            if (j > 0) {
                                var b = GetVertexByLoopIndex(i, j - 1);
                                var c = GetVertexByLoopIndex(i - 1, j - 1);
                                triangles.Add(new Triangle(vertex, b, c));

                                var b1 = GetVertexByLoopIndex(i - 1, j - 1);
                                var c1 = GetVertexByLoopIndex(i - 2, j);
                                triangles.Add(new Triangle(vertex, b1, c1));
                            }

                            if (j < verticalCount - 1) {
                                var b = GetVertexByLoopIndex(i - 2, j);
                                var c = GetVertexByLoopIndex(i - 1, j);
                                triangles.Add(new Triangle(vertex, b, c));
                            }
                        }
                    }
                }
            }

            return (vertices, triangles);

            Vertex GetVertexByLoopIndex(int i, int j) {
                var gridIndex = _subdivideCells
                    ? i / 2 * (verticalCount * 2 - 1) + i % 2 * verticalCount + j
                    : i * verticalCount + j;
                return vertices[gridIndex];
            }
        }

        private class Vertex {
            public readonly int Index;
            public Vector3 RelativePosition;
            public Vector3 Position { get; private set; }

            public readonly List<Triangle> Triangles = new List<Triangle>();

            public Vertex(IList<Vertex> vertexList) {
                Index = vertexList.Count;
                vertexList.Add(this);
            }

            public void SetPosition(Vector3 cellSize, Vector3 offset) {
                Position = Vector3.Scale(cellSize, RelativePosition + offset);
            }

            public Vector3 CalculateNormal() {
                if (Triangles.Count == 0) {
                    return -Vector3.forward;
                }

                return Triangles.Aggregate(Vector3.zero,
                    (acc, tri) => acc + tri.CalculateNormal() * tri.CalculateArea()) / Triangles.Count;
            }
        }

        private class Triangle {
            public readonly Vertex A;
            public readonly Vertex B;
            public readonly Vertex C;

            public Triangle(Vertex a, Vertex b, Vertex c) {
                A = a;
                A.Triangles.Add(this);
                B = b;
                B.Triangles.Add(this);
                C = c;
                C.Triangles.Add(this);
            }

            public Vector3 CalculateNormal() {
                return Vector3.Cross(B.Position - A.Position, C.Position - A.Position).normalized;
            }

            public float CalculateArea() {
                return Math.Abs(0.5f * ((B.Position.x - A.Position.x) * (C.Position.y - A.Position.y) -
                                        (C.Position.x - A.Position.x) * (B.Position.y - A.Position.y)));
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DiggingSurface))]
    public class DiggingSurfaceEditor : Editor {
        public override void OnInspectorGUI() {
            var surface = (DiggingSurface) target;

            base.OnInspectorGUI();
            if (GUI.changed) {
                surface.GenerateMeshEditor(false);
            }

            if (GUILayout.Button("Recalculate offsets")) {
                surface.GenerateMeshEditor(true);
            }
        }
    }
#endif
}