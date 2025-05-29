using _Game.Scripts.Data.SerializedReference;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Editor.SerializedReference {
    [CustomPropertyDrawer(typeof(SerializeReferenceMenuAttribute))]
    public class SerializeReferenceMenuDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var actualType = property.GetManagedReferenceActualType();

            if (PropertyDrawerProxy.TryFind(property, out var drawer)) {
                drawer.OnGUI(position, property, label);
            } else {
                DrawButton(ref position, property, ((SerializeReferenceMenuAttribute) attribute).AllowNull);
                EditorGUI.PropertyField(position, property,
                    new GUIContent($"{label.text} ({actualType.Name})"), true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            PropertyDrawerProxy.GetPropertyHeight(property, label);

        public static void DrawButton(ref Rect position, SerializedProperty property, bool allowNull) {
            if (property.serializedObject.isEditingMultipleObjects) {
                return;
            }

            var buttonPosition = new Rect(
                position.x + position.width - EditorGUIUtility.singleLineHeight,
                position.y,
                EditorGUIUtility.singleLineHeight,
                EditorGUIUtility.singleLineHeight);

            position = new Rect(position.x, position.y, position.width - EditorGUIUtility.singleLineHeight - 2.0f,
                position.height);

            var backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;

            var labelWidth = EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 15.0f;
            var labelRect = new Rect(position) {
                width = labelWidth,
                height = EditorGUIUtility.singleLineHeight
            };
            var e = Event.current;
            if (GUI.Button(buttonPosition, new GUIContent("...")) ||
                (e.type == EventType.MouseDown && labelRect.Contains(e.mousePosition) && e.button == 2)) {
                property.ShowContextMenuForManagedReference(allowNull);
            }

            GUI.backgroundColor = backgroundColor;
        }
    }
}