using System;
using System.Collections.Generic;
using System.Linq;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Game.Level.DynamicTerrain {
    public class DynamicMeshCell : IDynamicMeshCell {
        private const int RelativeSize = IDynamicMeshCell.RelativeSize;

        public Vector3Int RelativePosition { get; }
        public Vector3Int Index { get; }

        private readonly List<DynamicTriangle> _triangles = new List<DynamicTriangle>();

        public DynamicMeshCell(Vector3Int relativePosition, Vector3Int index) {
            RelativePosition = relativePosition;
            Index = index;
        }

        public void RegisterVertices(Dictionary<Vector3Int, DynamicVertex> pointDict) {
            GenerateDynamicMesh(RelativePosition, pointDict);
        }

        public Mesh GenerateMesh(Func<DynamicVertex, Vector3> getPosition, Quaternion rotation, Vector3 scale,
            IUVSettings uvSettings = null) {
            var mesh = new Mesh();
            mesh.vertices = _triangles
                .SelectMany(tri => Enumerable.Empty<Vector3>()
                    .Append(getPosition(tri.A))
                    .Append(getPosition(tri.B))
                    .Append(getPosition(tri.C)))
                .ToArray();
            mesh.normals = _triangles
                .SelectMany(tri => Enumerable.Repeat(tri.CalculateNormal(getPosition), 3))
                .ToArray();
            mesh.triangles = Enumerable.Range(0, mesh.vertices.Length).ToArray();
            if (uvSettings != null) {
                mesh.SetUVs(0, GenerateUVs(uvSettings));
            }

            return mesh;
        }

        private void GenerateDynamicMesh(Vector3Int relativePosition, Dictionary<Vector3Int, DynamicVertex> pointDict) {
            var origin = relativePosition - Vector3Int.one * RelativeSize / 2;
            var sides = new[] {
                (origin, Vector3Int.right, Vector3Int.up),
                (origin + Vector3Int.right * RelativeSize, Vector3Int.forward, Vector3Int.up),
                (origin + (Vector3Int.forward + Vector3Int.up) * RelativeSize, Vector3Int.right, Vector3Int.down),
                (origin + Vector3Int.forward * RelativeSize, Vector3Int.back, Vector3Int.up),
                (origin + Vector3Int.up * RelativeSize, Vector3Int.right, Vector3Int.forward),
                (origin + Vector3Int.forward * RelativeSize, Vector3Int.right, Vector3Int.back),
            };

            foreach (var (start, right, up) in sides) {
                GenerateSide(start, right, up, pointDict);
            }
        }

        private void GenerateSide(Vector3Int start, Vector3Int right, Vector3Int up,
            Dictionary<Vector3Int, DynamicVertex> pointDict) {
            for (var i = 0; i < 4; i++) {
                var a = GetVertex(start);
                var b = GetVertex(start + right * RelativeSize / 2 + up * RelativeSize / 2);
                var c = GetVertex(start + right * RelativeSize);

                _triangles.Add(new DynamicTriangle(a, b, c));

                start += right * RelativeSize;
                (right, up) = (up, -right);
            }

            DynamicVertex GetVertex(Vector3Int position) {
                var vertex = pointDict.GetValue(position, new DynamicVertex {
                    RelativePosition = position,
                });

                vertex.AddRelatedCell(true);
                return vertex;
            }
        }

        private List<Vector2> GenerateUVs(IUVSettings uvSettings) {
            var uvs = new List<Vector2>();

            foreach (var (start, right, up, size) in uvSettings.UVData) {
                GenerateUVSide(start, right, up, size, uvs);
            }

            return uvs;
        }

        private void GenerateUVSide(Vector2 start, Vector2 right, Vector2 up, Vector2 uvSizeSize, IList<Vector2> uvs) {
            for (var i = 0; i < 4; i++) {
                var aUV = start;
                var bUV = start + right * uvSizeSize.x / 2 + up * uvSizeSize.y / 2;
                var cUV = start + right * uvSizeSize.x;

                uvs.Add(aUV);
                uvs.Add(bUV);
                uvs.Add(cUV);

                start += right * uvSizeSize.x;
                (right, up) = (up, -right);
                uvSizeSize = new Vector2(uvSizeSize.y, uvSizeSize.x);
            }
        }

        public void Dispose() { }
    }
}