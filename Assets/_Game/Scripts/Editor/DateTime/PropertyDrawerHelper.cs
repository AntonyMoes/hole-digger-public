using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Editor.DateTime {
    public static class PropertyDrawerHelper {
        public static void Draw(Rect position, SerializedProperty property, GUIContent label, ref int? year,
            ref int? month, ref int day, ref int hour, ref int minute, ref int second) {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var next = position;
            var w = 16f + position.width / 16.0f;
            var dw = 20f;
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            next = new Rect(next.x, next.y, w, next.height);
            next = DrawOptionalPart(ref year, next, dw, w, "Y, ");
            next = DrawOptionalPart(ref month, next, dw, w, "M, ");
            next = DrawPart(ref day, next, dw, w, "D, ");
            next = DrawPart(ref hour, next, dw, w, "h, ");
            next = DrawPart(ref minute, next, dw, w, "m, ");
            DrawPart(ref second, next, dw, w, "s, ");

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private static Rect DrawOptionalPart(ref int? value, Rect rect, float fieldSize, float labelSize,
            string label) {
            if (value is not { } v) {
                return rect;
            }

            var result = DrawPart(ref v, rect, fieldSize, labelSize, label);
            value = v;
            return result;
        }

        private static Rect DrawPart(ref int value, Rect rect, float fieldSize, float labelSize, string label) {
            value = EditorGUI.IntField(rect, value);
            rect = new Rect(rect.x + labelSize, rect.y, fieldSize, rect.height);
            EditorGUI.LabelField(rect, label);

            return new Rect(rect.x + fieldSize, rect.y, labelSize, rect.height);
        }
    }
}