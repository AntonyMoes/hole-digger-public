using System;
using System.Collections.Generic;

namespace _Game.Scripts.DI {
    public class ApplicationContainer : IContainer, IContainerBuilder, IDisposable {
        private readonly bool _allowLazy;
        private readonly InstancesContainer _instances;
        private readonly DependencyGraph _dependencyGraph = new();
        private readonly HashSet<object> _createdDependencies = new();

        private bool _disposed;

        public ApplicationContainer(bool allowLazy = false) : this(allowLazy, null) { }

        private ApplicationContainer(bool allowLazy, InstancesContainer parent = null) {
            _allowLazy = allowLazy;
            _instances = new InstancesContainer(this, parent);
            AddInstance(this);
            AddInstance<IContainer>(this);
        }

        public int Version { get; private set; }

        public T Create<T>(params object[] context) {
            if (_allowLazy) CreateDependencies(typeof(T), context);

            return _instances.Create<T>(context);
        }

        public object Create(Type type, params object[] context) {
            if (_allowLazy) CreateDependencies(type, context);

            return _instances.Create(type, context);
        }

        public T Get<T>() {
            if (!_allowLazy) return _instances.Get<T>();

            var type = typeof(T);
            if (!_instances.Contains(type)) CreateDependencies(type);

            return _instances.Get<T>();
        }

        public void AddInstance<T>(T instance, params object[] context) {
            _dependencyGraph.AddInstance(instance);
            _instances.AddInstance(instance, context);
        }

        public void AddType<T, TInstance>() where TInstance : T {
            _dependencyGraph.AddType<T, TInstance>();
        }

        public TInstance CreateInstance<T, TInstance>(params object[] context) where TInstance : T {
            var instance = _instances.CreateInstance<T, TInstance>(context);
            _dependencyGraph.AddType<T, TInstance>();
            return instance;
        }

        internal bool TryGet(Type type, out object instance) {
            if (!_allowLazy) return _instances.TryGet(type, out instance);

            if (!_instances.Contains(type)) CreateDependencies(type);

            return _instances.TryGet(type, out instance);
        }

        public bool TryGet<T>(out T instance) {
            if (TryGet(typeof(T), out var o)) {
                instance = (T) o;
                return true;
            }

            instance = default;
            return false;
        }

        public bool TryGetOnly<T>(out T instance) {
            return _instances.TryGet(out instance);
        }

        public IContainer CreateChildContainer(Action<IContainerBuilder> callback) {
            var container = new ApplicationContainer(_allowLazy, _instances);
            callback?.Invoke(container);

            if (!_allowLazy) container.CreateAllTypes();

            return container;
        }

        public void Dispose() {
            if (_disposed) {
                return;
            }

            _disposed = true;
            _instances.Dispose();
        }

        public void CreateAllTypes() {
            _dependencyGraph.CreateAllTypes(_instances);
        }

        private void CreateDependencies(Type type, params object[] context) {
            if (_createdDependencies.Contains(type)) return;

            if (_dependencyGraph.CreateDependencies(type, _instances, context)) _createdDependencies.Add(type);
        }
    }
}