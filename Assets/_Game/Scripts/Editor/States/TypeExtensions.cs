using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEditor;

namespace _Game.Scripts.Editor.States {
    public static class TypeExtensions {
        private static IEnumerable<Type> _typesCache;
        private static readonly IDictionary<Type, Type[]> ImplementationsCache = new Dictionary<Type, Type[]>();

        public static string GetDisplayName(this Type type) {
            if (GetCustomDisplayName(type, out var name)) {
                return name;
            }

            var typeName = type.Name;
            if (type.IsAssignedFrom<IBaseImpact>()) {
                var splitName = typeName.Split(new[] {
                    "Impact"
                }, StringSplitOptions.None);
                return ObjectNames.NicifyVariableName(splitName.LastOrDefault());
            }

            if (type.IsAssignedFrom<ComponentTypeWrapper>()) {
                var index = typeName.IndexOf("Wrapper", StringComparison.InvariantCulture);
                return ObjectNames.NicifyVariableName(typeName.Remove(index));
            }

            return typeName;
        }

        public static string GetFullDisplayName(this Type type) {
            return GetCustomDisplayName(type, out var name) ? name : ObjectNames.NicifyVariableName(type.Name);
        }

        private static bool GetCustomDisplayName(Type type, out string name) {
            if (type.GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault()
                is DisplayNameAttribute attr) {
                name = attr.DisplayName;
                return true;
            }

            name = string.Empty;
            return false;
        }

        public static bool IsAssignedFrom<T>(this Type type) where T : class {
            return typeof(T).IsAssignableFrom(type);
        }

        private static bool ValidateType(Type type, Type parent) {
            return parent.IsAssignableFrom(type) && !type.IsAbstract;
        }

        public static Type GetFinalElementType(this Type type) {
            if (type.IsArray) {
                type = type.GetElementType();
            } else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
                type = type.GetGenericArguments().Single();
            }

            return type;
        }

        public static Type[] GetImplementationTypes(this Type type) {
            type = GetFinalElementType(type);
            if (type == null) {
                return null;
            }

            if (ImplementationsCache.TryGetValue(type, out var types)) {
                return types;
            }

            _typesCache ??= AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());

            var list = _typesCache.Where(target => ValidateType(target, type)).ToArray();
            ImplementationsCache.Add(type, list);

            return list;
        }
    }
}