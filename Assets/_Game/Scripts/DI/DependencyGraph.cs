using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine.Pool;

namespace _Game.Scripts.DI {
    public class DependencyGraph {
        private readonly Dictionary<Type, Node> _graphNodes = new();
        private readonly List<TypeInfo> _types = new();
        private readonly Dictionary<Type, List<Type>> _instanceTypeToKeyTypes = new();

        public void AddType<T, TInstance>() where TInstance : T {
            AddType(typeof(T), typeof(TInstance), true);
        }

        public void AddInstance<T>(T instance) {
            AddType(typeof(T), instance.GetType(), false);
        }

        private void AddType(Type keyType, Type instanceType, bool includeDependencies) {
            var dependencies = includeDependencies ? GetDependencies(instanceType) : Array.Empty<Type>();
            AddNode(keyType, instanceType, dependencies);
        }

        public void CreateAllTypes(InstancesContainer container) {
            using var _ = HashSetPool<Node>.Get(out var visitedNodes);
            using var __ = ListPool<TypeInfo>.Get(out var toCreateTypes);

            foreach (var type in _types)
                CollectTypesToCreate(_graphNodes[type.KeyType], visitedNodes, container, toCreateTypes);

            foreach (var type in toCreateTypes) {
                if (!container.Contains(type.KeyType)) Create(type, container);
            }
        }

        public IEnumerator CreateAllTypesRoutine(InstancesContainer container, int periodMs) {
            var allTypes = _types.Select(t => t.KeyType);
            yield return CreateTypesRoutine(allTypes, container, periodMs);
        }

        public IEnumerator CreateTypesRoutine(IEnumerable<Type> types, InstancesContainer container, int periodMs) {
            var visitedNodes = new HashSet<Node>(_graphNodes.Count);
            var toCreateTypes = new List<TypeInfo>(_graphNodes.Count);

            foreach (var type in types) CollectTypesToCreate(_graphNodes[type], visitedNodes, container, toCreateTypes);

            var time = Stopwatch.StartNew();

            foreach (var type in toCreateTypes) {
                if (container.Contains(type.KeyType)) continue;

                Create(type, container);
                if (time.ElapsedMilliseconds < periodMs) continue;

                time.Restart();
                yield return null;
            }
        }

        public bool CreateDependencies(Type keyType, InstancesContainer container, params object[] context) {
            if (_graphNodes.TryGetValue(keyType, out var node)) {
                foreach (var type in node.Dependencies) CreateTypes(type, container);

                CreateTypes(keyType, container);
                return true;
            }

            if (!keyType.IsAbstract && !keyType.IsInterface) {
                foreach (var type in GetDependencies(keyType)) {
                    if (container.Contains(type) || context.Any(obj => type.IsInstanceOfType(obj))) continue;

                    var parent = container.Parent?.ApplicationContainer;
                    if (parent == null || !parent.TryGet(type, out _)) CreateTypes(type, container);
                }

                return true;
            }

            return false;
        }

        public void CreateTypes(Type keyType, InstancesContainer container) {
            if (!_graphNodes.TryGetValue(keyType, out var node)) throw new Exception($"Not found type {keyType}");

            using var _ = HashSetPool<Node>.Get(out var visitedNodes);
            using var __ = ListPool<TypeInfo>.Get(out var toCreateTypes);

            CollectTypesToCreate(node, visitedNodes, container, toCreateTypes);

            foreach (var type in toCreateTypes) {
                if (!container.Contains(type.KeyType)) Create(type, container);
            }
        }

        private void Create(TypeInfo typeInfo, InstancesContainer container) {
            if (TryGetCreatedWithOtherInterface(container, typeInfo.InstanceType, out var instance)) {
                container.AddInstance(typeInfo.KeyType, instance);
                return;
            }

            container.CreateInstance(typeInfo.KeyType, typeInfo.InstanceType);
        }

        private bool TryGetCreatedWithOtherInterface(InstancesContainer container, Type type, out object instance) {
            instance = default;

            if (!_instanceTypeToKeyTypes.TryGetValue(type, out var keyTypes)) return false;

            foreach (var keyType in keyTypes) {
                if (container.TryGet(keyType, out instance)) return true;
            }

            return false;
        }

        private static void CollectTypesToCreate(Node node, ICollection<Node> visitedNodes,
            InstancesContainer container,
            ICollection<TypeInfo> target, Type parentType = null) {
            if (visitedNodes.Contains(node) || container.Contains(node.TypeInfo.KeyType)) return;

            visitedNodes.Add(node);

            if (node.IsEmpty) throw new Exception($"Not found dependency {node.TypeInfo.KeyType} for {parentType}");

            foreach (var adjacentNode in node.AdjacentNodes)
                CollectTypesToCreate(adjacentNode, visitedNodes, container, target, node.TypeInfo.InstanceType);

            if (!TypeHelper.IsInstant(node.TypeInfo.InstanceType)) target.Add(node.TypeInfo);
        }

        private void AddNode(Type keyType, Type instanceType, IEnumerable<Type> dependencies) {
            if (_instanceTypeToKeyTypes.TryGetValue(instanceType, out var keyTypes))
                keyTypes.Add(keyType);
            else
                _instanceTypeToKeyTypes[instanceType] = new List<Type>(1) { keyType };

            if (_graphNodes.TryGetValue(keyType, out var node)) {
                if (!node.IsEmpty)
                    throw new Exception($"Found duplicates: key type: {keyType}, instance type: {instanceType}");

                node.TypeInfo.InstanceType = instanceType;
                node.Dependencies = dependencies.ToArray();
            } else {
                node = new Node(new TypeInfo(keyType, instanceType), dependencies.ToArray());
                _graphNodes.Add(keyType, node);
                _types.Add(node.TypeInfo);
            }

            foreach (var dependencyKeyType in node.Dependencies) {
                if (!_graphNodes.TryGetValue(dependencyKeyType, out var adjacentNode)) {
                    adjacentNode = new Node(dependencyKeyType);
                    _graphNodes.Add(dependencyKeyType, adjacentNode);
                    _types.Add(adjacentNode.TypeInfo);
                }

                node.AdjacentNodes.Add(adjacentNode);
            }
        }

        private static IEnumerable<Type> GetDependencies(Type type) {
            using var _ = HashSetPool<Type>.Get(out var visited);
            using var __ = ListPool<Type>.Get(out var dependencies);
            CollectDependencies(type, visited, dependencies);
            return dependencies.Distinct().ToArray();
        }

        private static void CollectDependencies(Type type, ICollection<Type> visited, ICollection<Type> result) {
            if (visited.Contains(type)) return;

            visited.Add(type);

            var constructor = GetInjectConstructor(type);
            if (constructor == null) throw new Exception($"Type {type} does not have inject constructor");

            foreach (var parameter in constructor.GetParameters()) {
                if (TypeHelper.IsInstant(parameter.ParameterType))
                    CollectDependencies(parameter.ParameterType, visited, result);
                else if (!TypeHelper.IsProvider(parameter.ParameterType)) result.Add(parameter.ParameterType);
            }
        }

        private static ConstructorInfo GetInjectConstructor(Type type) {
            var constructor = type.GetConstructors().FirstOrDefault(c => c.IsDefined<InjectAttribute>());
            if (constructor == null) constructor = type.GetConstructor(Type.EmptyTypes);
            return constructor;
        }
    }

    public class Node {
        public TypeInfo TypeInfo { get; }
        public IReadOnlyCollection<Type> Dependencies { get; internal set; }
        public readonly List<Node> AdjacentNodes = new();
        public bool IsEmpty => TypeInfo.InstanceType == null;

        public Node(TypeInfo typeInfo, IReadOnlyCollection<Type> dependencies) {
            TypeInfo = typeInfo;
            Dependencies = dependencies;
        }

        public Node(Type keyType) : this(new TypeInfo(keyType, null), Array.Empty<Type>()) { }
    }

    public class TypeInfo {
        public Type KeyType { get; }

        public Type InstanceType { get; internal set; }

        public TypeInfo(Type keyType, Type instanceType) {
            KeyType = keyType;
            InstanceType = instanceType;
        }

        public override string ToString() {
            return $"KeyType: {KeyType}, InstanceType: {InstanceType}";
        }
    }
}