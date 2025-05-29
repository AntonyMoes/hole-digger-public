using System;
using System.Text.RegularExpressions;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Editor.States {
    [CustomPropertyDrawer(typeof(IBaseImpact<>), true)]
    public class ImpactSelectorDrawer : ImplementationSelectorDrawer {
        protected override object CreateReference(Type type) {
            return Activator.CreateInstance(type);
        }

        protected override Rect DrawSelector(Rect position,
            SerializedProperty property,
            ref GUIContent label,
            out bool warn) {
            warn = false;
            var match = Regex.Match(property.propertyPath, @"\[([0-9]{1,})\]$");
            if (match.Success) {
                var index = int.Parse(match.Groups[1].Value);
                label = new GUIContent($"Element {index}");

                var value = property.managedReferenceValue;
                if (index > 0 && value != null) {
                    var array = property.GetArrayProperty();
                    var detectType = value.GetType();
                    for (var i = 0; i < index; i++) {
                        var element = array.GetArrayElementAtIndex(i);
                        if (detectType != element.managedReferenceValue?.GetType()) {
                            continue;
                        }

                        warn = true;
                        break;
                    }
                }
            }

            return new Rect(position) { height = EditorGUIUtility.singleLineHeight };
        }
    }
}