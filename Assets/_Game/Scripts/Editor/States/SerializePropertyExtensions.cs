using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

namespace _Game.Scripts.Editor.States {
    public static class SerializeReferenceExtensions {
        private const string ArrayPropertySubstring = ".Array.data[";
        private const BindingFlags FindFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        public static Type GetValueType(this SerializedProperty property) {
            return ExtractTypeFromString(property?.managedReferenceFullTypename ?? string.Empty);
        }

        public static Type GetFieldType(this SerializedProperty property) {
            return ExtractTypeFromString(property?.managedReferenceFieldTypename ?? string.Empty);
        }

        private static Type ExtractTypeFromString(string typeName) {
            if (string.IsNullOrEmpty(typeName)) {
                return null;
            }

            var splitFieldTypename = typeName.Split(' ');
            var assemblyName = splitFieldTypename[0];
            var subStringTypeName = splitFieldTypename[1];
            var assembly = Assembly.Load(assemblyName);
            var targetType = assembly.GetType(subStringTypeName);

            return targetType;
        }

        public static bool IsArrayElement(this SerializedProperty property) {
            return property.propertyPath.Contains(ArrayPropertySubstring);
        }

        public static SerializedProperty GetArrayProperty(this SerializedProperty property) {
            return property.FindProperty(ArrayPropertySubstring);
        }

        public static SerializedProperty GetParentProperty(this SerializedProperty property) {
            return property.FindProperty(".Impacts");
        }

        private static SerializedProperty FindProperty(this SerializedProperty property, string searchString) {
            var path = property.propertyPath;
            var index = path.LastIndexOf(searchString, StringComparison.InvariantCulture);
            var targetName = path.Remove(index);
            return property.serializedObject.FindProperty(targetName);
        }

        public static object GetTargetObject(this SerializedProperty prop) {
            if (prop == null) {
                return null;
            }

            object obj = prop.serializedObject.targetObject;
            var elements = prop.propertyPath
                .Replace(ArrayPropertySubstring, "[")
                .Split('.');
            foreach (var element in elements) {
                if (element.Contains("[")) {
                    var indexStr = element.IndexOf("[", StringComparison.Ordinal);
                    var index = Convert.ToInt32(element[indexStr..].Replace("[", "").Replace("]", ""));
                    obj = GetValueInternal(obj, element[..indexStr], index);
                } else {
                    obj = GetValueInternal(obj, element);
                }
            }

            return obj;
        }

        private static object GetValueInternal(object source, string name) {
            if (source == null) {
                return null;
            }

            var type = source.GetType();
            while (type != null) {
                var field = type.GetField(name, FindFlags);
                if (field != null) {
                    return field.GetValue(source);
                }

                var property = type.GetProperty(name, FindFlags | BindingFlags.IgnoreCase);
                if (property != null) {
                    return property.GetValue(source, null);
                }

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValueInternal(object source, string name, int index) {
            if (GetValueInternal(source, name) is not IEnumerable enumerable) {
                return null;
            }

            var enm = enumerable.GetEnumerator();
            for (var i = 0; i <= index; i++) {
                if (!enm.MoveNext()) {
                    return null;
                }
            }

            return enm.Current;
        }
    }
}