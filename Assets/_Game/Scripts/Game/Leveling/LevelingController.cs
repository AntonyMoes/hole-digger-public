using System;
using System.Collections.Generic;
using _Game.Scripts.Data;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.DI;
using GeneralUtils;

namespace _Game.Scripts.Game.Leveling {
    public class LevelingController : ILevelingController {
        private const string DataKey = "Leveling";

        private readonly IDataStorage _dataStorage;
        private readonly ListData<LevelData> _rawData;
        private readonly Dictionary<LeveledEntityConfig, LevelingData> _data =
            new Dictionary<LeveledEntityConfig, LevelingData>();

        [Inject]
        public LevelingController(IDataStorage dataStorage) {
            _dataStorage = dataStorage;
            _rawData = dataStorage.GetData<ListData<LevelData>>(DataKey);
        }

        public ILevelingData GetLevelData(LeveledEntityConfig config) {
            return GetInternalLevelData(config);
        }

        private LevelingData GetInternalLevelData(LeveledEntityConfig config) {
            return _data.GetValue(config, () => new LevelingData(config, _rawData.GetItem(config.ConfigId), Save));
        }

        public bool CanAddLevel(LeveledEntityConfig config) {
            var data = GetInternalLevelData(config);
            return data.Level.Value < data.MaxLevel;
        }

        public bool TryAddLevel(LeveledEntityConfig config) {
            if (!CanAddLevel(config)) {
                return false;
            }

            var data = GetInternalLevelData(config);
            data.MutableLevel.Value += 1;
            return true;
        }

        private void Save() {
            _dataStorage.SetData(_rawData, DataKey);
        }

        [Serializable]
        private class LevelData : ListData<LevelData>.IItem {
            public int configId;
            public int level;

            public int ConfigId {
                get => configId;
                set => configId = value;
            }
        }

        private class LevelingData : ILevelingData {
            private readonly LeveledEntityConfig _config;
            private readonly LevelData _data;
            private readonly Action _save;

            public readonly UpdatedValue<int> MutableLevel = new UpdatedValue<int>();
            public IUpdatedValue<int> Level => MutableLevel;
            public int MaxLevel => _config.LevelsInfo.Count - 1;

            public LevelingData(LeveledEntityConfig config, LevelData data, Action save) {
                _config = config;
                _data = data;
                _save = save;

                MutableLevel.Value = data.level;
                MutableLevel.Subscribe(UpdateLevel);
            }

            private void UpdateLevel(int level) {
                _data.level = level;
                _save();
            }
        }
    }
}