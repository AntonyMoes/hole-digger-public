using System;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Editor.States {
    [CustomPropertyDrawer(typeof(ComponentTypeWrapper))]
    public class WrapperSelectorDrawer : ImplementationSelectorDrawer {
        private const float SelectorMaxWidth = 150f;
        private const float SelectorMinWidth = 70f;
        private const float SelectorPadding = 2f;

        protected override object CreateReference(Type type) {
            return Activator.CreateInstance(type);
        }

        protected override Rect DrawSelector(Rect position,
            SerializedProperty property,
            ref GUIContent label,
            out bool warn) {
            var selectWidth = Mathf.Clamp(EditorGUIUtility.labelWidth, SelectorMinWidth, SelectorMaxWidth);
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var descRect = new Rect {
                x = position.x + selectWidth + SelectorPadding,
                y = position.y,
                height = lineHeight,
                width = position.width - selectWidth - SelectorPadding
            };

            warn = false;
            label = GUIContent.none;
            EditorGUI.LabelField(descRect, property.managedReferenceValue?.ToString());

            return new Rect(position) {
                height = lineHeight,
                width = selectWidth
            };
        }
    }
}