using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace _Game.Scripts.DI {
    public static class TypeExtensions {
        public static bool IsAssignableFromGeneric(this Type generic, Type type) =>
            IsAssignableFromGeneric(generic, type, out _);

        public static bool IsAssignableFromGeneric(this Type generic, Type type, out Type[] arguments) {
            while (true) {
                if (!generic.IsGenericTypeDefinition) {
                    arguments = generic.IsGenericType ? generic.GetGenericArguments() : new[] { generic };
                    return generic.IsAssignableFrom(type);
                }

                var interfaceTypes = type.GetInterfaces();
                foreach (var it in interfaceTypes)
                    if (it.IsGenericType && it.GetGenericTypeDefinition() == generic) {
                        arguments = it.GetGenericArguments();
                        return true;
                    }

                if (type.IsGenericType && type.GetGenericTypeDefinition() == generic) {
                    arguments = type.GetGenericArguments();
                    return true;
                }

                var baseType = type.BaseType;
                if (baseType == null) {
                    arguments = null;
                    return false;
                }

                type = baseType;
            }
        }

        public static bool IsSubclassOrSameGeneric(this Type type, Type generic) =>
            type.IsSubclassOrSameGeneric(generic, out _);

        public static bool IsSubclassOrSameGeneric(this Type type, Type generic, out Type[] arguments) {
            if (!generic.IsGenericTypeDefinition) {
                arguments = Type.EmptyTypes;
                if (!type.IsGenericParameter) return type == generic || type.IsSubclassOf(generic);
                do {
                    if (type == generic || type.IsSubclassOf(generic)) return true;
                    type = type.BaseType;
                } while (type != null);

                return false;
            }

            while (type != null) {
                var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (generic == cur) {
                    arguments = type.GetGenericArguments();
                    return true;
                }

                type = type.BaseType;
            }

            arguments = null;
            return false;
        }

        public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider provider, bool inherit = true)
            where T : Attribute {
            return provider switch {
                Assembly assembly => Attribute.GetCustomAttributes(assembly, typeof(T), inherit).Cast<T>(),
                MemberInfo memberInfo => Attribute.GetCustomAttributes(memberInfo, typeof(T), inherit).Cast<T>(),
                Module module => Attribute.GetCustomAttributes(module, typeof(T), inherit).Cast<T>(),
                ParameterInfo parameterInfo => Attribute.GetCustomAttributes(parameterInfo, typeof(T), inherit)
                    .Cast<T>(),
                null => Enumerable.Empty<T>(),
                _ => throw new ArgumentOutOfRangeException(nameof(provider))
            };
        }

        public static bool IsDefined<T>(this ICustomAttributeProvider provider, bool inherit = true)
            where T : Attribute {
            return provider switch {
                Assembly assembly => Attribute.GetCustomAttribute(assembly, typeof(T), inherit) != null,
                MemberInfo memberInfo => Attribute.GetCustomAttribute(memberInfo, typeof(T), inherit) != null,
                Module module => Attribute.GetCustomAttribute(module, typeof(T), inherit) != null,
                ParameterInfo parameterInfo => Attribute.GetCustomAttribute(parameterInfo, typeof(T), inherit) != null,
                null => false,
                _ => throw new ArgumentOutOfRangeException(nameof(provider))
            };
        }

        public static T GetAttribute<T>(this ICustomAttributeProvider provider, bool inherit = true)
            where T : Attribute {
            return provider switch {
                Assembly assembly => (T) Attribute.GetCustomAttribute(assembly, typeof(T), inherit),
                MemberInfo memberInfo => (T) Attribute.GetCustomAttribute(memberInfo, typeof(T), inherit),
                Module module => (T) Attribute.GetCustomAttribute(module, typeof(T), inherit),
                ParameterInfo parameterInfo => (T) Attribute.GetCustomAttribute(parameterInfo, typeof(T), inherit),
                null => null,
                _ => throw new ArgumentOutOfRangeException(nameof(provider))
            };
        }

        public static bool TryGetAttribute<T>(this ICustomAttributeProvider provider, out T attribute,
            bool inherit = true) where T : Attribute {
            attribute = provider.GetAttributes<T>(inherit).FirstOrDefault();
            return attribute != null;
        }
    }
}