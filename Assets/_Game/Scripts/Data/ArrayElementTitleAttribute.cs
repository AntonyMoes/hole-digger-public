using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Game.Scripts.Data {
    public class ArrayElementTitleAttribute : PropertyAttribute {
        public readonly string VariableName;

        public ArrayElementTitleAttribute(string variableName) {
            VariableName = variableName;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ArrayElementTitleAttribute))]
    public class ArrayElementTitleDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        protected virtual ArrayElementTitleAttribute Attribute => (ArrayElementTitleAttribute) attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var titleProperty = property.FindPropertyRelative(Attribute.VariableName);
            var newLabel = GetTitle(titleProperty);
            if (string.IsNullOrEmpty(newLabel)) {
                newLabel = label.text;
            }

            EditorGUI.PropertyField(position, property, new GUIContent(newLabel, label.tooltip), true);
        }

        private static string GetTitle(SerializedProperty property) {
            if (property == null) {
                return "";
            }

            return property.propertyType switch {
                SerializedPropertyType.Integer => property.intValue.ToString(),
                SerializedPropertyType.Boolean => property.boolValue.ToString(),
                SerializedPropertyType.Float => property.floatValue.ToString(),
                SerializedPropertyType.String => property.stringValue,
                SerializedPropertyType.Color => property.colorValue.ToString(),
                SerializedPropertyType.ObjectReference => property.objectReferenceValue != null
                    ? property.objectReferenceValue.ToString()
                    : "",
                SerializedPropertyType.Enum => property.enumNames[property.enumValueIndex],
                SerializedPropertyType.Vector2 => property.vector2Value.ToString(),
                SerializedPropertyType.Vector3 => property.vector3Value.ToString(),
                SerializedPropertyType.Vector4 => property.vector4Value.ToString(),
                SerializedPropertyType.Rect => property.rectValue.ToString(),
                SerializedPropertyType.Vector2Int => property.vector2IntValue.ToString(),
                SerializedPropertyType.Vector3Int => property.vector3IntValue.ToString(),
                SerializedPropertyType.RectInt => property.rectIntValue.ToString(),
                SerializedPropertyType.ArraySize => "",
                SerializedPropertyType.Character => "",
                SerializedPropertyType.AnimationCurve => "",
                SerializedPropertyType.Bounds => "",
                SerializedPropertyType.BoundsInt => "",
                SerializedPropertyType.Gradient => "",
                SerializedPropertyType.Quaternion => "",
                SerializedPropertyType.ExposedReference => "",
                SerializedPropertyType.FixedBufferSize => "",
                SerializedPropertyType.ManagedReference => "",
                SerializedPropertyType.LayerMask => "",
                SerializedPropertyType.Generic => "",
                _ => ""
            };
        }
    }
#endif
}