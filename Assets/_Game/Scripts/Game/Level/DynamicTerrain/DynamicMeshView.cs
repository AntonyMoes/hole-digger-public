using System;
using System.Collections.Generic;
using System.Linq;
using GeneralUtils;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Game.Level.DynamicTerrain {
    public class DynamicMeshView : MonoBehaviour, IDynamicMeshView {
        [SerializeField] private DynamicMeshCellView _cellView;

        [Header("Settings")]
        [Range(0, 20)] [SerializeField] private float _width;
        [Range(0, 20)] [SerializeField] private float _height;
        [Range(0, 20)] [SerializeField] private float _depth;
        [Range(1, 16)] [SerializeField] private int _widthCells;
        [Range(1, 16)] [SerializeField] private int _heightCells;
        [Range(1, 16)] [SerializeField] private int _depthCells;
        [Tooltip("Offset relative to cell size")]
        [Range(0, 0.5f)] [SerializeField] private float _offset;

        private readonly Rng _rng = new Rng();
        private readonly Dictionary<Vector3Int, Vector3> _relativeOffsets = new Dictionary<Vector3Int, Vector3>();
        private readonly Dictionary<Vector3Int, DynamicVertex> _vertices = new Dictionary<Vector3Int, DynamicVertex>();
        private readonly Dictionary<DynamicMeshCellView, Row> _cells = new Dictionary<DynamicMeshCellView, Row>();
        private readonly Dictionary<int, List<IDynamicMeshCell>> _dynamicMeshCellRows =
            new Dictionary<int, List<IDynamicMeshCell>>();
        private readonly List<Row> _rows = new List<Row>();
        private IMeshCellProvider _cellProvider;
        private IMeshCellViewProvider _viewProvider;

        public IDictionary<DynamicMeshCellView, Row> Cells => _cells;
        public int DepthCells => _depthCells;

        public Vector3 CellSize => new Vector3(_width / _widthCells, _height / _heightCells, _depth / _depthCells);
        public Vector3Int MeshSizeInCells => new Vector3Int(_widthCells, _heightCells, _depthCells);
        private Vector3Int RelativeOrigin => -(MeshSizeInCells - Vector3Int.one) * IDynamicMeshCell.RelativeSize / 2;

        public Vector3 BackBoundary => transform.position +
                                       GetRelativePosition(new Vector3(0, 0,
                                           +RelativeOrigin.z +
                                           (_rows[0].Index - 0.5f) * IDynamicMeshCell.RelativeSize));

        public Row FirstRow => _rows[0];

        private void OnDrawGizmos() {
            if (_rows.Count > 0) {
                Gizmos.DrawSphere(BackBoundary, 1f);
            }
        }

        public void Init(IMeshCellProvider cellProvider, IMeshCellViewProvider viewProvider, int initialDepth) {
            _cellProvider = cellProvider;
            _viewProvider = viewProvider;
            UpdateMesh(initialDepth);
        }

        public void Clear() {
            foreach (var cell in _cells.Keys) {
                DestroyCell(cell);
            }

            _dynamicMeshCellRows.Clear();
            _vertices.Clear();
            _cells.Clear();
            _rows.Clear();

#if UNITY_EDITOR
            if (!Application.isPlaying) {
                for (var i = 0; i < transform.childCount; i++) {
                    var child = transform.GetChild(i);
                    if (child.gameObject.activeSelf && child.TryGetComponent<DynamicMeshCellView>(out var cell)) {
                        DestroyCell(cell);
                    }
                }
            }
#endif
        }

        public void Remove(DynamicMeshCellView cell) {
            var row = _cells[cell];
            row.Remove(cell);

            _cells.Remove(cell);
            DestroyCell(cell);
        }

        private void DestroyCell(DynamicMeshCellView cell) {
            var immediate = !Application.isPlaying;
            if (immediate) {
                DestroyImmediate(cell.gameObject);
            } else {
                Destroy(cell.gameObject);
            }
        }

        public DynamicMeshCellView GetFrontCellInLine(Vector3Int position) {
            foreach (var row in _rows) {
                var rowPosition = position;
                rowPosition.z = RelativeOrigin.z + row.Index * IDynamicMeshCell.RelativeSize;
                if (row.Cells.TryGetValue(rowPosition, out var result)) {
                    return result;
                }
            }

            throw new ArgumentOutOfRangeException($"NO CELL IN LINE {position}");
        }

        public Vector3Int GetCellIndex(Vector3Int relativePosition) {
            return (relativePosition - RelativeOrigin) / IDynamicMeshCell.RelativeSize;
        }

#if UNITY_EDITOR
        public void GenerateMeshEditor(bool regenerateOffsets) {
            if (regenerateOffsets) {
                _relativeOffsets.Clear();
            }

            UpdateMesh(0);
        }
#endif

        private void UpdateMesh(int initialDepth) {
            Clear();

            for (var i = 0; i < _depthCells; i++) {
                GenerateRow(i + initialDepth);
            }
        }

        public void GenerateNewRow() {
            GenerateRow(_rows.Last().Index + 1);
        }

        private void GenerateRow(int rowIndex) {
            var row = new Row(rowIndex, _rows);

            var cells = GenerateDynamicRow(rowIndex);
            GenerateDynamicRow(rowIndex + 1);
            var vertexPositions = new Dictionary<DynamicVertex, Vector3>();

            foreach (var cell in cells) {
                var view = _viewProvider?.InstantiateView(cell.RelativePosition, cell.Index, transform)
                           ?? Instantiate(_cellView, transform);
                view.Init(cell, vertex => GetPosition(vertex) - GetRelativePosition(cell.RelativePosition));
                view.transform.localPosition = GetRelativePosition(cell.RelativePosition);
                _cells.Add(view, row);
                row.Add(view);
            }

            Vector3 GetPosition(DynamicVertex vertex) {
                return vertexPositions.GetValue(vertex, () => {
                    var offset = _relativeOffsets.GetValue(vertex.RelativePosition, () =>
                        vertex.CanBeOffset
                            ? new Vector3(
                                _rng.NextFloat(-1, 1),
                                _rng.NextFloat(-1, 1),
                                _rng.NextFloat(-1, 1)
                            )
                            : Vector3.zero);

                    var relativePosition = vertex.RelativePosition + offset * (_offset * IDynamicMeshCell.RelativeSize);
                    return GetRelativePosition(relativePosition);
                });
            }
        }

        private List<IDynamicMeshCell> GenerateDynamicRow(int rowIndex) {
            if (_dynamicMeshCellRows.TryGetValue(rowIndex, out var row)) {
                return row;
            }

            var cells = new List<IDynamicMeshCell>();
            for (var i = 0; i < _widthCells; i++) {
                for (var j = 0; j < _heightCells; j++) {
                    var index = new Vector3Int(i, j, rowIndex);
                    var offset = index * IDynamicMeshCell.RelativeSize;
                    var relativePosition = RelativeOrigin + offset;
                    var cell = _cellProvider != null
                        ? _cellProvider.GetCell(relativePosition, index)
                        : new DynamicMeshCell(relativePosition, index);

                    if (cell == null) {
                        continue;
                    }

                    cell.RegisterVertices(_vertices);
                    cells.Add(cell);
                }
            }

            _dynamicMeshCellRows.Add(rowIndex, cells);
            return cells;
        }

        public Vector3 GetRelativePosition(Vector3 relativeVertexPosition) {
            return Vector3.Scale(relativeVertexPosition, CellSize / IDynamicMeshCell.RelativeSize);
        }

        public class Row {
            public readonly int Index;

            private readonly Dictionary<Vector3Int, DynamicMeshCellView> _cells =
                new Dictionary<Vector3Int, DynamicMeshCellView>();
            private readonly List<Row> _rows;

            public IReadOnlyDictionary<Vector3Int, DynamicMeshCellView> Cells => _cells;
            public int RelativeDepth => _rows.IndexOf(this);

            public Row(int index, List<Row> rows) {
                Index = index;
                _rows = rows;
                rows.Add(this);
            }

            public void Add(DynamicMeshCellView cell) {
                _cells.Add(cell.Cell.RelativePosition, cell);
            }

            public void Remove(DynamicMeshCellView cell) {
                _cells.Remove(cell.Cell.RelativePosition);
                cell.Clear();
                if (_cells.Count == 0) {
                    _rows.Remove(this);
                }
            }
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(DynamicMeshView))]
    public class DynamicMeshViewEditor : Editor {
        public override void OnInspectorGUI() {
            var view = (DynamicMeshView) target;

            base.OnInspectorGUI();
            if (GUI.changed) {
                view.GenerateMeshEditor(false);
            }

            if (GUILayout.Button("Build")) {
                view.GenerateMeshEditor(false);
            }

            if (GUILayout.Button("Recalculate offsets")) {
                view.GenerateMeshEditor(true);
            }

            if (GUILayout.Button("Clear")) {
                view.Clear();
            }
        }
    }
#endif
}