using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Resource;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Game.Level {
    public class LevelController : ILevelController {
        private const string DataKey = "Level";

        private readonly IDataStorage _dataStorage;
        private readonly LevelData _data;
        private readonly IResourceController _resourceController;

        public UpdatedValue<bool> PrioritizeResources { get; } = new UpdatedValue<bool>();

        public UpdatedValue<int> Depth { get; } = new UpdatedValue<int>();

        public UpdatedValue<IReadOnlyList<ILevelController.LevelCell>> LevelCells { get; } =
            new UpdatedValue<IReadOnlyList<ILevelController.LevelCell>>();

        private readonly UpdatedValue<Dictionary<Vector3Int, ILevelController.LevelCell>> _levelCellsDictionary =
            new UpdatedValue<Dictionary<Vector3Int, ILevelController.LevelCell>>();
        public IUpdatedValue<Dictionary<Vector3Int, ILevelController.LevelCell>> LevelCellsDictionary =>
            _levelCellsDictionary;

        private readonly Event<IResourceValue> _collectDropEvent = new Event<IResourceValue>();
        public IEvent<IResourceValue> CollectDropEvent => _collectDropEvent;

        [Inject]
        public LevelController(IDataStorage dataStorage, IResourceController resourceController) {
            _dataStorage = dataStorage;
            _data = dataStorage.GetData<LevelData>(DataKey);
            _resourceController = resourceController;
            Depth.Value = _data.depth;
            LevelCells.Value = _data.cells.Select(cell => new ILevelController.LevelCell {
                Index = cell.cellIndex,
                OreConfigId = cell.oreConfigId.ToNullable()
            }).ToArray();
            Depth.Subscribe(UpdateDepth);
            LevelCells.Subscribe(UpdateCells);
            LevelCells.Subscribe(UpdateCellDictionary, true);
        }

        public bool TryCollectDrop(IResourceValue value, out CantAddReason reason) {
            if (!_resourceController.TryAdd(value, out reason)) {
                return false;
            }

            _collectDropEvent.Invoke(value);
            return true;
        }

        private void UpdateDepth(int depth) {
            _data.depth = depth;
            Save();
        }

        private void UpdateCells(IReadOnlyList<ILevelController.LevelCell> cells) {
            _data.cells = cells.Select(cell => new LevelCell {
                cellIndex = cell.Index,
                oreConfigId = cell.OreConfigId.ToOptional()
            }).ToArray();
            Save();
        }

        private void UpdateCellDictionary(IReadOnlyList<ILevelController.LevelCell> cells) {
            _levelCellsDictionary.Value = cells.ToDictionary(cell => cell.Index, cell => cell);
        }

        private void Save() {
            _dataStorage.SetData(_data, DataKey);
        }

        [Serializable]
        private class LevelData {
            public int depth;
            public LevelCell[] cells = Array.Empty<LevelCell>();
        }

        [Serializable]
        private class LevelCell {
            public Vector3Int cellIndex;
            public Optional<int> oreConfigId;
        }
    }
}