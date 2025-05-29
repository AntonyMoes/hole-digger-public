using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Editor.States {
    public abstract class ImplementationSelectorDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var color = GUI.color;
            {
                var implementTypes = property.GetFieldType().GetImplementationTypes();
                var currentType = property.managedReferenceFullTypename;
                var curIndex = !string.IsNullOrEmpty(currentType)
                    ? implementTypes.Select(i => i.FullName).ToList().IndexOf(currentType.Split(' ')[1]) + 1
                    : 0;

                var names = implementTypes.Select(t => new GUIContent(t.GetDisplayName()))
                    .Prepend(new GUIContent("Not implemented")).ToArray();

                var rect = DrawSelector(position, property, ref label, out var warn);

                GUI.color = warn ? Color.yellow : curIndex == 0 ? Color.red : Color.green;
                var newIndex = EditorGUI.Popup(rect, label, curIndex, names);
                GUI.color = color;

                if (newIndex != curIndex) {
                    property.managedReferenceValue = newIndex == 0
                        ? null
                        : CreateReference(implementTypes[newIndex - 1]);
                }
            }

            EditorGUI.PropertyField(position, property, GUIContent.none, true);
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        protected abstract object CreateReference(Type type);

        protected abstract Rect DrawSelector(Rect position,
            SerializedProperty property,
            ref GUIContent label,
            out bool warn);
    }
}