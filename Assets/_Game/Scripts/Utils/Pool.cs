using System;
using System.Collections.Generic;

namespace _Game.Scripts.Utils {
    public class Pool<T> where T : class {
        private readonly List<Record> _objects = new List<Record>();
        private readonly Func<T> _creator;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;

        public Pool(Func<T> creator, int initialSize = 0, Action<T> onGet = null, Action<T> onRelease = null) {
            _creator = creator;
            _onGet = onGet;
            _onRelease = onRelease;

            for (var i = 0; i < initialSize; i++) {
                CreateRecord();
            }
        }

        public IHandler Get() {
            foreach (var record in _objects) {
                if (!record.Used) {
                    return new Handler(record, _onGet, _onRelease);
                }
            }

            var newRecord = CreateRecord();
            return new Handler(newRecord, _onGet, _onRelease);
        }

        private Record CreateRecord() {
            var record = new Record(_creator(), false);
            _objects.Add(record);
            return record;
        }

        private class Record {
            public readonly T Object;
            public bool Used;

            public Record(T obj, bool used) {
                Object = obj;
                Used = used;
            }
        }

        private class Handler : IHandler {
            private Record _record;
            private readonly Action<T> _onRelease;

            public T Object => _record?.Object;

            public Handler(Record record, Action<T> onGet, Action<T> onRelease) {
                _record = record;
                _onRelease = onRelease;

                _record.Used = true;
                onGet?.Invoke(record.Object);
            }

            public void Release() {
                if (_record != null) {
                    _record.Used = false;
                    _onRelease?.Invoke(_record.Object);
                    _record = null;
                }
            }
        }

        public interface IHandler {
            public T Object { get; }
            public void Release();
        }
    }
}