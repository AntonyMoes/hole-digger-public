using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data;
using _Game.Scripts.Data.Configs.Condition;
using _Game.Scripts.DI;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Condition {
    public class ConditionDataController : IConditionDataController {
        private const string DataKey = "Conditions";

        private readonly IDataStorage _dataStorage;
        private readonly ListData<ConditionDataRecord> _data;
        private readonly Dictionary<int, IConditionData> _trackedConditions;

        [Inject]
        public ConditionDataController(ConditionsConfig config, IDataStorage dataStorage, IContainer container) {
            _dataStorage = dataStorage;
            _data = dataStorage.GetData<ListData<ConditionDataRecord>>(DataKey);
            _trackedConditions = config.TrackedDataConfigs
                .ToDictionary(condition => condition.ConfigId, condition => {
                    var data = condition.GetValue(container);
                    var recordStorage = new ConditionDataStorage(_data.GetItem(condition.ConfigId), Save);
                    data.Init(recordStorage);
                    return data;
                });
        }

        public IUpdatedValue<TData> GetConditionData<TData>(int configId) {
            if (!_trackedConditions.TryGetValue(configId, out var rawData)) {
                throw new Exception($"The condition data with configId {configId} is not tracked!");
            }

            if (rawData is not IConditionData<TData> data) {
                throw new Exception(
                    $"The condition data with configId {configId} is not of type {typeof(IConditionData<TData>)}; it is {rawData.GetType()}");
            }

            return data.Data;
        }

        private void Save() {
            _dataStorage.SetData(_data, DataKey);
        }

        [Serializable]
        private class ConditionDataRecord : ListData<ConditionDataRecord>.IItem {
            public int configId;
            public string data = "";

            public int ConfigId {
                get => configId;
                set => configId = value;
            }
        }

        private class ConditionDataStorage : IConditionDataStorage {
            private readonly ConditionDataRecord _record;
            private readonly Action _save;

            public ConditionDataStorage(ConditionDataRecord record, Action save) {
                _record = record;
                _save = save;
            }

            public TData Load<TData>() {
                var savedData = JsonUtility.FromJson<DataWrapper<TData>>(_record.data);
                if (savedData == null) {
                    return default;
                }

                return savedData.data;
            }

            public void Save<TData>(TData data) {
                _record.data = JsonUtility.ToJson(new DataWrapper<TData> { data = data });
                _save();
            }

            [Serializable]
            private class DataWrapper<TData> {
                public TData data;
            }
        }
    }
}