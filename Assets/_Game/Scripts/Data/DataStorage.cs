using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.DI;
using _Game.Scripts.Scheduling;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Data {
    public class DataStorage : IDataStorage {
        private const string DataKey = "STORAGE_DATA";
        private const float FlushPeriod = 1f;

        private readonly StorageData _data;
        private readonly Dictionary<string, object> _parsedData = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _modifiedData = new Dictionary<string, object>();
        private readonly IDisposable _flushToken;
        private readonly IDisposable _quitToken;
        private bool _needFlush;
        private bool _disposed;

        public readonly bool New;

        [Inject]
        public DataStorage(ITimeEventProvider scheduler) {
            _data = Load(out New);
            _flushToken = scheduler.SubscribeToPeriodicEvent(Flush, FlushPeriod);
            _quitToken = scheduler.QuitEvent.Subscribe(Flush);
        }

        /// <summary>
        /// Deletes all player data.
        /// Game should be completely reloaded after this.
        /// </summary>
        public void Reset() {
            _disposed = true;
            DeleteData();
        }

#if UNITY_EDITOR
        public static void EditorReset() {
            DeleteData();
        }

        public string EditorLoadData() {
            return LoadString();
        }

        public void EditorSetData(string data) {
            SetString(data);
        }
#endif

        private static void DeleteData() {
            PlayerPrefs.DeleteKey(DataKey);
        }

        public void Dispose() {
            _flushToken?.Dispose();
            _quitToken?.Dispose();
        }

        private static StorageData Load(out bool created) {
            var dataString = LoadString();
            created = string.IsNullOrEmpty(dataString);
            return created
                ? new StorageData()
                : JsonUtility.FromJson<StorageData>(dataString);
        }

        private static string LoadString() {
            return PlayerPrefs.GetString(DataKey, null);
        }

        private void Save() {
            _needFlush = true;
        }

        public T GetData<T>(string key) where T : new() {
            return (T) (_modifiedData.TryGetValue(key, out var data)
                ? data
                : _parsedData.GetValue(key, () => {
                    var storedData = _data.data.FirstOrDefault(d => d.key == key) ??
                                     new StorageDataItem { key = key };
                    return string.IsNullOrEmpty(storedData.data) ? new T() : JsonUtility.FromJson<T>(storedData.data);
                }));
        }

        public void SetData<T>(T data, string key) {
            _modifiedData[key] = data;

            var item = _data.data.FirstOrDefault(i => i.key == key);
            if (item == null) {
                item = new StorageDataItem {
                    key = key
                };
                _data.data.Add(item);
            }
            item.data = JsonUtility.ToJson(data);

            Save();
        }

        public void ForceSave() {
            Flush();
        }

        private void Flush() {
            if (!_needFlush) {
                return;
            }

            _needFlush = false;
            if (_disposed) {
                return;
            }

            var dataString = JsonUtility.ToJson(_data);
            SetString(dataString);
        }

        private void SetString(string data) {
            PlayerPrefs.SetString(DataKey, data);
            
        }

        [Serializable]
        private class StorageData {
            public List<StorageDataItem> data = new List<StorageDataItem>();
        }

        [Serializable]
        private class StorageDataItem {
            public string key;
            public string data;
        }
    }
}