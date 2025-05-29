using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data.Configs.Level;
using _Game.Scripts.Utils;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Game.Level.DynamicTerrain {
    public class PrebuiltMeshCell : IDynamicMeshCell {
        private readonly List<DynamicVertex> _dynamicVertices = new List<DynamicVertex>();
        private readonly PrebuiltMeshConfig.PointDeformData[] _deformData;
        private readonly Pool<Mesh>.IHandler _meshHandler;

        public static readonly Vector3Int[] DeformableVertices = new Vector3Int[] {
            -Vector3Int.one,
            new Vector3Int(1, -1, -1),
            new Vector3Int(1, 1, -1),
            new Vector3Int(-1, 1, -1),
            new Vector3Int(-1, -1, 1),
            new Vector3Int(1, -1, 1),
            Vector3Int.one,
            new Vector3Int(-1, 1, 1),
        };

        public Vector3Int RelativePosition { get; }
        public Vector3Int Index { get; }

        public PrebuiltMeshCell(Vector3Int relativePosition, Vector3Int index, PrebuiltMeshConfig.PointDeformData[] deformData,
            Pool<Mesh>.IHandler meshHandler) {
            RelativePosition = relativePosition;
            Index = index;
            _deformData = deformData;
            _meshHandler = meshHandler;
        }

        public void RegisterVertices(Dictionary<Vector3Int, DynamicVertex> pointDict) {
            foreach (var vertex in DeformableVertices) {
                RegisterVertex(vertex, true);
            }

            RegisterVertex(new Vector3Int(-1, 0, 0));
            RegisterVertex(new Vector3Int(1, 0, 0));
            RegisterVertex(new Vector3Int(0, -1, 0));
            RegisterVertex(new Vector3Int(0, 1, 0));
            RegisterVertex(new Vector3Int(0, 0, -1));
            RegisterVertex(new Vector3Int(0, 0, 1));

            void RegisterVertex(Vector3Int relativePosition, bool canOffset = false) {
                var position = RelativePosition + relativePosition * IDynamicMeshCell.RelativeSize / 2;
                var vertex = pointDict.GetValue(position, new DynamicVertex {
                    RelativePosition = position,
                });
                vertex.AddRelatedCell(canOffset);

                if (canOffset) {
                    _dynamicVertices.Add(vertex);
                }
            }
        }

        public Mesh GenerateMesh(Func<DynamicVertex, Vector3> getPosition, Quaternion rotation, Vector3 scale,
            IUVSettings uvSettings = null) {
            var vertexToActualPosition = _dynamicVertices.ToDictionary(
                vertex => Vector3Int.RoundToInt(Quaternion.Inverse(rotation) * (vertex.RelativePosition - RelativePosition)),
                vertex => Quaternion.Inverse(rotation) * getPosition(vertex));

            var newVertices = _meshHandler.Object.vertices.ToArray();
            var scaleInverted = scale.Inverted();
            foreach (var deformData in _deformData) {
                var from = vertexToActualPosition[deformData.fromPoint];
                var to = vertexToActualPosition[deformData.toPoint];
                var position = deformData.distance * (to - from) + from;
                newVertices[deformData.index] = Vector3.Scale(position + deformData.offset, scaleInverted);
            }

            _meshHandler.Object.SetVertices(newVertices);
            return _meshHandler.Object;
        }

        public void Dispose() {
            _meshHandler.Release();
        }
    }
}