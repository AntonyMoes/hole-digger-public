using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Audio;
using _Game.Scripts.Data.Configs.Level;
using _Game.Scripts.Data.Configs.Meta.ResourceValue;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Level.Digging.Tools;
using _Game.Scripts.Game.Level.DynamicTerrain;
using _Game.Scripts.Utils;
using _Game.Scripts.Vibration;
using GeneralUtils;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Game.Scripts.Game.Level.Digging {
    // TODO: create a proper view
    public class LevelDiggingController : IMeshCellViewProvider, IMeshCellProvider, IDisposable {
        private readonly Rng _rng = new Rng();
        private readonly Dictionary<int, List<CellView>> _rows = new Dictionary<int, List<CellView>>();
        private readonly Dictionary<Drop, Pool<Drop>.IHandler> _usedDrops = new Dictionary<Drop, Pool<Drop>.IHandler>();
        private readonly Dictionary<ITool, IToolView> _toolViews = new Dictionary<ITool, IToolView>();
        private readonly Dictionary<Vector3Int, ICellData> _cells = new Dictionary<Vector3Int, ICellData>();

        private readonly ILevelDiggingView _view;
        private readonly Dictionary<OreConfig, OreData> _oreData;
        private readonly CellView _defaultCellPrefab;
        private readonly CellView _frontRowCellPrefab;
        private readonly IDynamicMeshView _dynamicMeshView;
        private readonly IToolController _toolController;
        private readonly LevelConfig _levelConfig;
        private readonly IUpdatedValue<float> _cameraMovement;

        [CanBeNull] private IReadOnlyList<ILevelController.LevelCell> _savedCells;

        public IEnumerable<ILevelController.LevelCell> Cells => _cells.Select(pair => new ILevelController.LevelCell {
            Index = pair.Key,
            OreConfigId = pair.Value.OreConfig.NullSafe()?.ConfigId
        }).ToArray();

        [Inject]
        public LevelDiggingController(IDynamicMeshView dynamicMeshView, LevelConfig levelConfig,
            IToolController toolController, IReadOnlyList<ILevelController.LevelCell> savedCells,
            IUpdatedValue<float> cameraMovement, ILevelDiggingView view) {
            _view = view;
            _defaultCellPrefab = levelConfig.CellPrefab.GetComponent<CellView>();
            _frontRowCellPrefab = levelConfig.FrontRowCellPrefab.GetComponent<CellView>();
            _savedCells = savedCells.Count == 0 ? null : savedCells;
            _dynamicMeshView = dynamicMeshView;
            _toolController = toolController;
            _levelConfig = levelConfig;
            _cameraMovement = cameraMovement;

            _oreData = _levelConfig.OreGenerationConfig.Ores
                .ToDictionary(oreConfig => oreConfig, oreConfig => {
                    var oreCellPrefab = oreConfig.CellPrefab.GetComponent<OreCellView>();
                    var dropPrefab = oreConfig.DropPrefab.GetComponent<Drop>();
                    return new OreData {
                        Prefab = oreCellPrefab,
                        DropPool = new Pool<Drop>(() => {
                                var drop = Object.Instantiate(dropPrefab, _view.DropParent);
                                drop.gameObject.SetActive(false);
                                return drop;
                            }, 1,
                            drop => drop.gameObject.SetActive(true),
                            drop => drop.gameObject.SetActive(false)),
                        MeshPool = new Pool<Mesh>(() => Object.Instantiate(oreCellPrefab.MeshConfig.Mesh))
                    };
                });
        }

        public void ClearSavedCells() {
            _savedCells = null;
        }

        public IEnumerable<CellView> Dig(CellView cellView, int rowIndex) {
            var cellIndex = cellView.View.Cell.Index;
            var cellData = _cells[cellIndex];
            var cellSize = _dynamicMeshView.CellSize;
            var meanCellSize = Mathf.Sqrt(cellSize.x * cellSize.y);

            var tool = _toolController.GetTool(cellData);
            var toolView = GetToolView(tool);
            var heightRatio = (float) cellIndex.y / (_dynamicMeshView.MeshSizeInCells.y - 1);
            toolView.Play(cellView.transform.position, GetCellImpactPoint(cellView), heightRatio);
            VibrationController.Instance.Vibrate(VibrationType.Light);

            return tool.Dig(_rng, cellData, _cells)
                .Select(data => {
                    var view = _rows[rowIndex].Find(view => view.View.Cell.Index == data.Index);
                    if (view == default) {
                        throw new Exception(
                            $"Dug cell [Index {data.Index}] not found in row [Index {rowIndex}]");
                    }

                    if (data.StrikesRemaining > 0) {
                        _view.LevelEffectController.PlayPoof(GetCellImpactPoint(view), meanCellSize / 2f);
                        return default;
                    }

                    if (data.OreConfig is { } config) {
                        var oreData = _oreData[config];
                        for (var i = 0; i < data.DropCount; i++) {
                            SpawnDrop(cellView, config.Drop, oreData.DropPool);
                        }
                    }

                    return (data, view);
                })
                .Where(pair => pair != default)
                .Select(pair => {
                    _view.LevelEffectController.PlayPoof(pair.view.transform.position, meanCellSize);
                    _rows[rowIndex].Remove(pair.view);
                    _cells.Remove(pair.data.Index);
                    return pair.view;
                });
        }

        private void SpawnDrop(CellView cellView, ResourceValueConfig dropValue, Pool<Drop> dropPool) {
            var drop = dropPool.Get();
            drop.Object.Init(dropValue, CollectDrop, RemoveDrop);
            drop.Object.transform.position = cellView.transform.position;
            _usedDrops.Add(drop.Object, drop);
        }

        private void CollectDrop(Drop drop) {
            RemoveDrop(drop);
            AudioController.Instance.Play(_view.DropPickupSound);
            VibrationController.Instance.Vibrate(VibrationType.Medium);
        }

        private void RemoveDrop(Drop drop) {
            var handler = _usedDrops[drop];
            _usedDrops.Remove(drop);
            handler.Release();
        }

        private IToolView GetToolView(ITool tool) {
            return _toolViews.GetValue(tool, () => tool.CreateView(_view.ToolParent));
        }

        private Vector3 GetCellImpactPoint(CellView cellView) {
            var cell = cellView.transform;
            var cellSize = _dynamicMeshView.CellSize;
            return cell.position - Quaternion.Inverse(cell.transform.localRotation) *
                cell.transform.forward * cellSize.z / 2f;
        }

        public void AnimateNextRow(int rowIndex) {
            // TODO separate from digging
            foreach (var cell in _rows[rowIndex]) {
                cell.SubscribeToCameraMovement(_cameraMovement);
            }
        }

        public IDynamicMeshCell GetCell(Vector3Int relativePosition, Vector3Int index) {
            OreConfig oreConfig;
            if (_savedCells != null) {
                var savedCell = _savedCells.FirstOrDefault(savedCell => savedCell.Index == index);
                if (savedCell == null) {
                    return null;
                }

                oreConfig = _oreData.Keys.FirstOrDefault(config => config.ConfigId == savedCell.OreConfigId);
            } else {
                oreConfig = index.z == 0 && _levelConfig.SpecialFrontRow
                    ? null
                    : _levelConfig.OreGenerationConfig.GenerateOre(_rng);
            }

            var strikesToBreak = oreConfig.NullSafe()?.StrikesToBreak ?? _levelConfig.DefaultStrikesToBreak;
            _cells.Add(index, new CellData(index, strikesToBreak, oreConfig));
            return CreateMeshCell(relativePosition, index, oreConfig);
        }

        private IDynamicMeshCell CreateMeshCell(Vector3Int relativePosition, Vector3Int index,
            [CanBeNull] OreConfig oreConfig) {
            if (oreConfig == null) {
                return new DynamicMeshCell(relativePosition, index);
            }

            var oreData = _oreData[oreConfig];
            return new PrebuiltMeshCell(relativePosition, index, oreData.Prefab.MeshConfig.PointData,
                oreData.MeshPool.Get());
        }

        public DynamicMeshCellView InstantiateView(Vector3Int relativePosition, Vector3Int index, Transform parent) {
            var row = _rows.GetValue(index.z, () => new List<CellView>());

            if (index.z == 0 && _levelConfig.SpecialFrontRow) {
                var frontCell = Object.Instantiate(_frontRowCellPrefab, parent);
                frontCell.View.UVSettings = new FrontRowUVSettings(_dynamicMeshView.MeshSizeInCells, index);
                row.Add(frontCell);
                return frontCell.View;
            }

            var cellData = _cells[index];
            if (cellData.OreConfig == null) {
                var cell = Object.Instantiate(_defaultCellPrefab, parent);
                row.Add(cell);
                return cell.View;
            }

            var oreData = _oreData[cellData.OreConfig];
            var resourceCell = Object.Instantiate(oreData.Prefab, parent);
            resourceCell.Init(_rng, _dynamicMeshView.CellSize);
            row.Add(resourceCell.CellView);
            return resourceCell.CellView.View;
        }

        private class OreData {
            public OreCellView Prefab;
            public Pool<Drop> DropPool;
            public Pool<Mesh> MeshPool;
        }

        private class CellData : ICellData {
            public Vector3Int Index { get; }
            public int StrikesRemaining { get; set; }
            public int DropCount { get; set; } = 1;
            public OreConfig OreConfig { get; }

            public CellData(Vector3Int index, int strikesToBreak, [CanBeNull] OreConfig config) {
                Index = index;
                OreConfig = config;
                StrikesRemaining = strikesToBreak;
            }
        }

        public void Dispose() {
            foreach (var toolView in _toolViews.Values) {
                toolView.Dispose();
            }
        }
    }
}