using System;

namespace _Game.Scripts.DI {
    public struct LazyProvider<T> : IProvider<T> where T : class {
        private readonly ApplicationContainer _container;
        private T _instance;
        private int _version;

        public LazyProvider(ApplicationContainer container) {
            _instance = null;
            _container = container;
            _version = container.Version;
        }

        public T Instance {
            get {
                var version = _container.Version;
                if (_instance == null || _version != version) {
                    _instance = _container.Get<T>();
                    if (_instance == null) throw new NullReferenceException($"Dependency {typeof(T)} not found");
                    _version = version;
                }

                return _instance;
            }
        }

        public bool TryGet(out T instance) {
            var version = _container.Version;
            if (_instance != null && _version == version) {
                instance = _instance;
                return true;
            }


            if (_container.TryGet<T>(out var newInstance)) {
                instance = _instance = newInstance;
                _version = version;
                return true;
            }

            instance = default;
            return false;
        }
    }
}