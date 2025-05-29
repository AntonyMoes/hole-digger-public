using System;

namespace _Game.Scripts.DI {
    public static class TypeHelper {
        public static bool IsProvider(Type type) {
            return typeof(IProvider<>).IsAssignableFromGeneric(type);
        }

        public static bool IsProvider(Type type, out Type[] arguments) {
            return typeof(IProvider<>).IsAssignableFromGeneric(type, out arguments);
        }

        public static bool IsInstant(Type type) {
            return typeof(IInstant).IsAssignableFrom(type);
        }
    }
}