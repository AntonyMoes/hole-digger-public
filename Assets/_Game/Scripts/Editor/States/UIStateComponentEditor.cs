using _Game.Scripts.UI.States;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace _Game.Scripts.Editor.States {
    [CustomEditor(typeof(UIStateComponent), true)]
    [CanEditMultipleObjects]
    public class UIStateComponentEditor : UnityEditor.Editor {
        private SerializedProperty _header;
        private SerializedProperty _wrappers;
        private ReorderableList _list;

        private void OnEnable() {
            _wrappers = serializedObject.FindProperty("_wrappers");
            _header = serializedObject.FindProperty("DisplayName");
            _list = new ReorderableList(
                serializedObject,
                _wrappers,
                true,
                false,
                true,
                true) {
                drawElementCallback = DrawElementCallback,
                elementHeightCallback = ElementHeightCallback
            };
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            if (target is UIStateComponent component) {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(_header, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("▶", GUILayout.Width(20f))) {
                    component.Apply();
                }

                if (GUILayout.Button("▶*", GUILayout.Width(30f))) {
                    component.Apply();
                    EditorUtility.SetDirty(target);
                }

                EditorGUILayout.EndHorizontal();
            }

            var dropRect = EditorGUILayout.GetControlRect(false, 25f);
            GUI.Box(dropRect, "DROP OBJECT HERE", EditorStyles.centeredGreyMiniLabel);
            UIStateDrawer.DropAreaGUI(dropRect, _wrappers);

            _list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
            var element = _wrappers.GetArrayElementAtIndex(index);
            rect.x += 10f;
            rect.width -= 10f;
            EditorGUI.PropertyField(rect, element);
        }

        private float ElementHeightCallback(int index) {
            var element = _wrappers.GetArrayElementAtIndex(index);
            return element.isExpanded ? EditorGUI.GetPropertyHeight(element) : EditorGUIUtility.singleLineHeight;
        }
    }
}