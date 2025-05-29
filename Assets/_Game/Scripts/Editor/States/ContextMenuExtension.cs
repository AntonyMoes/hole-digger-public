using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using _Game.Scripts.UI.States;
using _Game.Scripts.UI.States.Impacts;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Game.Scripts.Editor.States {
    public static class ContextMenuExtension {
        private static (string json, Type type) _lastCopyObject;

        private static readonly GUIContent LabelCopy = new("Copy");
        private static readonly GUIContent LabelPaste = new("Paste");
        private static readonly GUIContent LabelDuplicate = new("Duplicate");
        private static readonly GUIContent LabelFill = new("Fill Default Values");
        private static readonly GUIContent LabelFillWrapper = new("Fill All Impact Default Values");

        private static readonly GUIContent LabelWrappers = new("UIState Wrappers");
        private static readonly GUIContent LabelImpacts = new("UIState Impacts");

        private static readonly List<Type> WrapperTypes = new();

        #region Context menu for UIState

        [DidReloadScripts]
        private static void OnScriptsReloaded() {
            EditorApplication.contextualPropertyMenu -= OnPropertyMenu;
            EditorApplication.contextualPropertyMenu += OnPropertyMenu;

            WrapperTypes.Clear();
            WrapperTypes.AddRange(typeof(ComponentTypeWrapper).GetImplementationTypes());
        }

        private static void OnPropertyMenu(GenericMenu menu, SerializedProperty property) {
            if (property.propertyType != SerializedPropertyType.ManagedReference) {
                return;
            }

            if (!property.IsArrayElement()) {
                return;
            }

            var isImpact = false;
            var type = property.GetValueType();

            if (!type.IsAssignedFrom<ComponentTypeWrapper>()) {
                isImpact = type.IsAssignedFrom<IBaseImpact>();
                if (!isImpact) {
                    return;
                }
            }

            menu.AddDisabledItem(isImpact ? LabelImpacts : LabelWrappers);

            var copyProperty = property.Copy();
            menu.AddItem(LabelCopy, false, _ => OnCopy(copyProperty), null);
            menu.AddItem(LabelPaste, false, _ => OnPaste(copyProperty), null);
            menu.AddItem(LabelDuplicate, false, _ => OnDuplicate(copyProperty), null);

            if (isImpact) {
                menu.AddItem(LabelFill, false, _ => OnFillImpact(copyProperty), null);
            } else {
                menu.AddItem(LabelFillWrapper, false, _ => OnFillWrapper(copyProperty), null);
            }
        }

        private static void OnCopy(SerializedProperty property) {
            var refValue = property.managedReferenceValue;
            _lastCopyObject.json = JsonUtility.ToJson(refValue);
            _lastCopyObject.type = refValue?.GetType();
        }

        private static void OnPaste(SerializedProperty property) {
            try {
                if (_lastCopyObject.type != null) {
                    var pasteObj = JsonUtility.FromJson(_lastCopyObject.json, _lastCopyObject.type);
                    property.managedReferenceValue = pasteObj;
                } else {
                    property.managedReferenceValue = null;
                }

                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
            } catch (Exception) {
                // ignored
            }
        }

        private static void OnDuplicate(SerializedProperty property) {
            var sourceElement = property.managedReferenceValue;
            var arrayProperty = property.GetArrayProperty();
            var newIndex = arrayProperty.arraySize;
            arrayProperty.arraySize = newIndex + 1;

            if (sourceElement != null) {
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();

                var json = JsonUtility.ToJson(sourceElement);
                var newObj = JsonUtility.FromJson(json, sourceElement.GetType());
                var newElementProperty = arrayProperty.GetArrayElementAtIndex(newIndex);
                newElementProperty.managedReferenceValue = newObj;
            }

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        private static void OnFillImpact(SerializedProperty property) {
            var parent = property.GetParentProperty();
            if (parent.managedReferenceValue is not ComponentTypeWrapper wrapper) {
                return;
            }

            var match = Regex.Match(property.propertyPath, @"\[([0-9]{1,})\]$");
            if (!match.Success) {
                return;
            }

            if (int.TryParse(match.Groups.LastOrDefault()?.Value, out var impactIndex)) {
                wrapper.FillDefaultValue(impactIndex);
            }
        }

        private static void OnFillWrapper(SerializedProperty property) {
            if (property.managedReferenceValue is not ComponentTypeWrapper wrapper) {
                return;
            }

            wrapper.FillAllDefaultValue();
        }

        #endregion

        #region Context menu for Drag&Drop

        public static void DragNDropMenu(SerializedProperty property, Object draggedObject) {
            if (EditorUtility.IsPersistent(draggedObject)) {
                return;
            }

            switch (draggedObject) {
                case GameObject gameObject: {
                    var components = gameObject.GetComponents<Component>();
                    CreateAndShowMenu(
                        draggedObject.name,
                        menu => {
                            AddDndGameObject(menu, property, gameObject);
                            foreach (var comp in components) {
                                AddDndMenuItem(menu, property, comp, true);
                            }
                        });
                    return;
                }
                case Component component: {
                    CreateAndShowMenu(
                        draggedObject.name + "." + component.GetType().Name,
                        menu => { AddDndMenuItem(menu, property, component, false); });
                    return;
                }
            }
        }

        private static void CreateAndShowMenu(string title, Action<GenericMenu> callback) {
            var menu = new GenericMenu();
            menu.AddDisabledItem(new GUIContent("Impacts for " + title));

            callback.Invoke(menu);

            if (menu.GetItemCount() == 1) {
                menu.AddDisabledItem(new GUIContent("No impacts"));
            }

            menu.ShowAsContext();
        }

        private static void AddDndGameObject(GenericMenu menu, SerializedProperty property, GameObject gameObject) {
            var type = typeof(GameObjectWrapper);
            var impacts = typeof(IGameObjectImpact).GetImplementationTypes();
            foreach (var impact in impacts) {
                menu.AddItem(
                    new GUIContent("GameObject/" + type.GetFullDisplayName() + "/" + impact.GetDisplayName()),
                    false,
                    OnDNDCreate,
                    (property, gameObject as Object, type, impact));
            }
        }

        private static List<(Type, Type)> GetPossibleTypes(Type type) {
            var list = new List<(Type, Type)>();
            foreach (var wrapperType in WrapperTypes) {
                var genTypes = wrapperType.BaseType?.GenericTypeArguments;
                if (genTypes?.Length > 0 && genTypes[0] == type) {
                    list.Add((wrapperType, genTypes[1]));
                }
            }

            if (type.BaseType != null && type.BaseType != type) {
                list.AddRange(GetPossibleTypes(type.BaseType));
            }

            return list;
        }

        private static void AddDndMenuItem(GenericMenu menu,
            SerializedProperty property,
            Component component,
            bool addComponentName) {
            var componentType = component.GetType();
            var componentName = addComponentName ? componentType.Name + "/" : "";
            if (componentType == typeof(InnerUIStateWrapper)) {
                menu.AddItem(
                    new GUIContent(componentName + typeof(InnerUIStateWrapper).GetFullDisplayName()),
                    false,
                    OnDNDCreate,
                    (property, component as Object, typeof(InnerUIStateWrapper)));
            }

            var possibleTypes = GetPossibleTypes(componentType);
            foreach (var (wrapperType, impactBase) in possibleTypes) {
                var impactsList = impactBase.GetImplementationTypes();
                foreach (var impact in impactsList) {
                    menu.AddItem(
                        new GUIContent(
                            componentName + wrapperType.GetFullDisplayName() + "/" +
                            impact.GetDisplayName()),
                        false,
                        OnDNDCreate,
                        (property, component as Object, wrapperType, impact));
                }
            }
        }

        private static void OnDNDCreate(object data) {
            var (property, component, wrapperType, impactType) =
                data as (SerializedProperty, Object, Type, Type)? ?? (null, null, null, null);

            var wrapper = Activator.CreateInstance(wrapperType);
            if (wrapper is InnerUIStateWrapper innerState) {
                innerState.InnerState = component as UIStateComponent;
            } else {
                SetValueToWrapper(wrapperType, wrapper, "Targets", component);
                if (impactType != null) {
                    var impact = Activator.CreateInstance(impactType);
                    var fillMethod = impactType.GetMethod("FillDefaultValues");
                    if (fillMethod != null) {
                        fillMethod.Invoke(impact, new object[] {
                            component
                        });
                    }

                    SetValueToWrapper(wrapperType, wrapper, "Impacts", impact);
                }
            }

            property.serializedObject.Update();
            property.arraySize += 1;

            var newElement = property.GetArrayElementAtIndex(property.arraySize - 1);
            newElement.managedReferenceValue = wrapper;
            newElement.isExpanded = true;

            property.serializedObject.ApplyModifiedProperties();

            var newElementTargets = newElement.FindPropertyRelative("Targets");
            newElementTargets.isExpanded = true;

            var newElementImpacts = newElement.FindPropertyRelative("Impacts");
            newElementImpacts.isExpanded = true;

            var newElementLastImpact = newElementImpacts.GetArrayElementAtIndex(0);
            newElementLastImpact.isExpanded = true;
        }

        private static void SetValueToWrapper(Type wrapperType, object wrapper, string fieldName, object value) {
            var field = wrapperType.GetField(fieldName);
            var arrayType = field.FieldType.GetElementType();
            if (arrayType == null) {
                throw new ArgumentException($"Field \"{fieldName}\" has null element type");
            }

            var array = Array.CreateInstance(arrayType, 1);
            array.SetValue(value, 0);
            field.SetValue(wrapper, array);
        }

        #endregion
    }
}