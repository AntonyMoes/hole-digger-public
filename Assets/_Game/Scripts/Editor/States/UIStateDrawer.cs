using _Game.Scripts.UI.States;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Editor.States {
    [CustomPropertyDrawer(typeof(UIState))]
    public class UIStateDrawer : PropertyDrawer {
        public static void DropAreaGUI(Rect dropArea, SerializedProperty property) {
            var @event = Event.current;
            switch (@event.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(@event.mousePosition) || DragAndDrop.objectReferences.Length > 1) {
                        return;
                    }

                    var draggedObject = DragAndDrop.objectReferences[0];
                    if (EditorUtility.IsPersistent(draggedObject)) {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        return;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (@event.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();
                        ContextMenuExtension.DragNDropMenu(property, draggedObject);
                    }

                    @event.Use();
                    break;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            var rect = new Rect(position);
            rect.width -= 23f;

            var descriptionProperty = property.FindPropertyRelative("_wrappers");
            var dropRect = new Rect(position) { height = EditorGUIUtility.singleLineHeight };
            DropAreaGUI(dropRect, descriptionProperty);
            EditorGUI.PropertyField(rect, descriptionProperty, label);
            GUI.Box(dropRect, "DROP OBJ HERE", EditorStyles.centeredGreyMiniLabel);

            rect.x += rect.width;
            rect.width = 20f;
            rect.height = EditorGUIUtility.singleLineHeight;

            if (GUI.Button(rect, "▶")) {
                var stateContainer = (UIState) property.GetTargetObject();
                stateContainer.Apply();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var descriptionProperty = property.FindPropertyRelative("_wrappers");
            return EditorGUI.GetPropertyHeight(descriptionProperty);
        }
    }
}