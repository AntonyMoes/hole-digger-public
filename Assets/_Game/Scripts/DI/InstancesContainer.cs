using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine.Pool;

namespace _Game.Scripts.DI {
    public class InstancesContainer : IDisposable {
        [CanBeNull] private readonly InstancesContainer _parent;

        private readonly ApplicationContainer _container;
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();
        private bool _released;

        internal ApplicationContainer ApplicationContainer => _container;
        internal InstancesContainer Parent => _parent;

        public InstancesContainer(ApplicationContainer container, InstancesContainer parent = null) {
            _container = container;
            _parent = parent;
        }

        public T Create<T>(params object[] context) => (T) Create(typeof(T), context);

        public T Get<T>() => (T) Get(typeof(T), null);

        public void AddInstance<T>(T instance, params object[] context) {
            InjectFields(instance, instance.GetType(), context);
            Add(typeof(T), instance);
        }

        public void AddInstance(Type type, object instance, params object[] context) {
            InjectFields(instance, instance.GetType(), context);
            Add(type, instance);
        }

        private void Add(Type type, object instance) {
            _instances.Add(type, instance);
        }

        public TInstance CreateInstance<T, TInstance>(params object[] context) where TInstance : T {
            return (TInstance) CreateInstance(typeof(T), typeof(TInstance), context);
        }

        public object CreateInstance(Type keyType, Type instanceType, params object[] context) {
            var instance = Create(instanceType, context);
            Add(keyType, instance);
            return instance;
        }


        public bool TryGet<T>(out T instance) {
            if (TryGet(typeof(T), out var o)) {
                instance = (T) o;
                return true;
            }

            instance = default;
            return false;
        }


        private object Get(Type type, Type targetType, params object[] context) {
            if (!TryGet(type, out var instance, context))
                throw new NullReferenceException($"Dependency {type} of {targetType} not found");
            return instance;
        }

        public bool Contains(Type type) {
            if (_released) return false;

            if (_instances.ContainsKey(type)) return true;

            return _parent != null && _parent.Contains(type);
        }

        public bool TryGet(Type type, out object instance, params object[] context) {
            if (_released) {
                instance = null;
                return false;
            }

            foreach (var param in context)
                if (type.IsInstanceOfType(param)) {
                    instance = param;
                    return true;
                }

            if (_instances.TryGetValue(type, out instance)) return true;

            if (_parent != null) {
                return _parent.TryGet(type, out instance, context);
            }

            return TryCreate(type, out instance, context);
        }

        private bool TryCreate(Type type, out object instance, object[] context) {
            if (!TypeHelper.IsInstant(type) || type.IsInterface || type.IsAbstract) {
                instance = null;
                return false;
            }

            instance = Create(type, context);
            Add(type, instance);
            return true;
        }

        public object Create(Type type, params object[] context) {
            foreach (var constructor in type.GetConstructors()) {
                if (!constructor.IsDefined<InjectAttribute>()) continue;
                var instance = InjectConstructor(type, constructor, context);
                return instance;
            }

            {
                var instance = Activator.CreateInstance(type);
                InjectFields(instance, instance.GetType(), context);
                return instance;
            }
        }

        private object GetProvider(Type type) {
            return Activator.CreateInstance(typeof(LazyProvider<>).MakeGenericType(type), _container);
        }

        private object InjectConstructor(Type type, MethodBase constructor, params object[] context) {
            using var _ = ListPool<object>.Get(out var parameters);

            foreach (var parameter in constructor.GetParameters()) {
                object value;
                if (TypeHelper.IsProvider(parameter.ParameterType, out var types))
                    value = GetProvider(types[0]);
                else {
                    var fieldType = parameter.ParameterType;
                    value = Get(fieldType, type, context);
                }

                parameters.Add(value);
            }

            try {
                var instance = Activator.CreateInstance(type, parameters.ToArray());
                return instance;
            } catch (TargetInvocationException e) {
                // Debug.LogException(e.InnerException);
                throw;
            }
        }

        private void ReleaseInject(object obj) {
            var type = obj.GetType();
            ReleaseInjectFields(obj, type);
        }

        private static void ReleaseInjectFields(object obj, Type type) {
            if (type.BaseType != null) ReleaseInjectFields(obj, type.BaseType);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var field in fields) {
                if (!field.IsDefined<InjectAttribute>()) continue;
                field.SetValue(obj, null);
            }
        }

        private void InjectFields(object obj, Type type, params object[] context) {
            if (type.BaseType != null) InjectFields(obj, type.BaseType, context);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var field in fields) {
                var isDependency = field.IsDefined<InjectAttribute>();
                if (!isDependency) continue;

                object value;
                if (TypeHelper.IsProvider(field.FieldType, out var types))
                    value = GetProvider(types[0]);
                else {
                    var fieldType = field.FieldType;
                    // if (TypeHelper.IsInstant(fieldType))
                    //     Debug.LogErrorFormat("Instant {0} injected in {1}.{2}", fieldType.Name, obj.GetType(),
                    //         field.Name);
                    value = Get(fieldType, type, context);
                }

                field.SetValue(obj, value);
            }
        }

        public void Dispose() {
            foreach (var instance in _instances.Values) {
                if (instance != null) {
                    if (instance is IDisposable disposable) {
                        disposable.Dispose();
                    }

                    try {
                        ReleaseInject(instance);
                    } catch (Exception e) {
                        // Debug.LogException(e);
                    }
                }
            }

            _instances.Clear();
            _released = true;
        }
    }
}