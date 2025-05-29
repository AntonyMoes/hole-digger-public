using System;
using System.Collections.Generic;
using System.Reflection;
using _Game.Scripts.DI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace _Game.Scripts.Editor.SerializedReference {
    public static class PropertyDrawerProxy {
        private static readonly GUIContent Text = new();

        private static readonly Dictionary<Type, Type> CustomDrawers = new();

        private static GUIContent TempContent(string t) {
            Text.image = null;
            Text.text = t;
            Text.tooltip = null;
            return Text;
        }

        public static float GetPropertyHeight(SerializedProperty property, GUIContent label = null) =>
            TryFind(property, out var drawer)
                ? drawer.GetPropertyHeight(property, label ?? TempContent(property.displayName))
                : EditorGUI.GetPropertyHeight(property, label ?? TempContent(property.displayName));

        public static void OnGUI(Rect position, SerializedProperty property, GUIContent label = null) {
            if (TryFind(property, out var drawer))
                drawer.OnGUI(position, property, label ?? TempContent(property.displayName));
            else
                EditorGUI.PropertyField(position, property, label ?? TempContent(property.displayName), true);
        }

        public static bool TryFind(SerializedProperty property, out PropertyDrawer drawer) {
            var scriptAttributeUtilityType =
                typeof(PropertyDrawer).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
            Assert.IsNotNull(scriptAttributeUtilityType);
            var getFieldInfoAndStaticTypeFromPropertyMethod =
                scriptAttributeUtilityType.GetMethod("GetFieldInfoAndStaticTypeFromProperty",
                    BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(getFieldInfoAndStaticTypeFromPropertyMethod);
            object[] parameters = {
                property,
                null
            };
            var fieldInfo = getFieldInfoAndStaticTypeFromPropertyMethod.Invoke(null, parameters);
            if (fieldInfo == null) {
                drawer = null;
                return false;
            }

            var propertyType = (Type) parameters[1];
            if (property.propertyType == SerializedPropertyType.ManagedReference)
                propertyType = property.GetManagedReferenceActualType();

            if (!CustomDrawers.TryGetValue(propertyType, out var drawerType)) {
                drawerType = Find(propertyType);
                CustomDrawers.Add(propertyType, drawerType);
            }

            if (drawerType == null) {
                drawer = null;
                return false;
            }

            var fieldInfoField =
                typeof(PropertyDrawer).GetField("m_FieldInfo", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(fieldInfoField);
            drawer = (PropertyDrawer) Activator.CreateInstance(drawerType);
            fieldInfoField.SetValue(drawer, fieldInfo);
            return drawer != null;
        }

        private static Type Find(Type propertyType) {
            var typeField =
                typeof(CustomPropertyDrawer).GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(typeField);
            var childField =
                typeof(CustomPropertyDrawer).GetField("m_UseForChildren",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(childField);
            var candidates = TypeCache.GetTypesWithAttribute<CustomPropertyDrawer>();
            foreach (var candidate in candidates) {
                var attributes = candidate.GetCustomAttributes<CustomPropertyDrawer>();
                foreach (var attribute in attributes) {
                    var drawerType = (Type) typeField.GetValue(attribute);
                    if ((drawerType == propertyType || ((bool) childField.GetValue(attribute) &&
                                                        propertyType.IsSubclassOrSameGeneric(drawerType))) &&
                        candidate.IsSubclassOf(typeof(PropertyDrawer)))
                        return candidate;
                }
            }

            return null;
        }
    }
}