using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace _Game.Scripts.Editor.SerializedReference {
    public static class SerializedPropertyExtensions {
        private static Dictionary<Type, IMenuItemFilter> _typeToFilter;

        private static void AssureTypeToFilterInitialized() {
            if (_typeToFilter != null) return;

            _typeToFilter = new Dictionary<Type, IMenuItemFilter>();
            var filterTypes = TypeCache.GetTypesDerivedFrom<IMenuItemFilter>()
                .Where(p => !p.IsAbstract);
            foreach (var filterType in filterTypes) {
                try {
                    var filter = (IMenuItemFilter) Activator.CreateInstance(filterType);
                    Assert.IsNotNull(filter);
                    Assert.IsNotNull(filter.ParentType);
                    _typeToFilter[filter.ParentType] = filter;
                } catch (Exception e) {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        public static void AssignNewInstanceOfTypeToManagedReference(this SerializedProperty serializedProperty,
            Type type) {
            var instance = Activator.CreateInstance(type);
            serializedProperty.serializedObject.UpdateIfRequiredOrScript();
            serializedProperty.managedReferenceValue = instance;
            serializedProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetManagedReferenceToNull(this SerializedProperty serializedProperty) {
            serializedProperty.serializedObject.UpdateIfRequiredOrScript();
            serializedProperty.managedReferenceValue = null;
            serializedProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public static IEnumerable<Type> GetAppropriateTypesForAssigningToManagedReference(
            this SerializedProperty property) {
            AssureTypeToFilterInitialized();

            var fieldType = property.GetManagedReferenceFieldType();
            _typeToFilter.TryGetValue(fieldType, out var filter);

            var derivedTypes = TypeCache.GetTypesDerivedFrom(fieldType);
            var results = derivedTypes.Where(type => !type.IsSubclassOf(typeof(Object)) && !type.IsAbstract);
            return filter != null
                ? filter.Handle(property, results)
                : results;
        }

        public static Type GetManagedReferenceFieldType(this SerializedProperty property) {
            var type = GetRealTypeFromTypename(property.managedReferenceFieldTypename);
            if (type != null) return type;

            throw new NotSupportedException(
                $"Can not get field type of managed reference : {property.managedReferenceFieldTypename}");
        }

        public static Type GetManagedReferenceActualType(this SerializedProperty property) {
            var type = GetRealTypeFromTypename(property.managedReferenceFullTypename);
            return type != null ? type : GetManagedReferenceFieldType(property);
        }

        public static Type GetRealTypeFromTypename(string stringType) {
            var names = GetSplitNamesFromTypename(stringType);
            var realType = Type.GetType($"{names.ClassName}, {names.AssemblyName}");
            return realType;
        }

        private static (string AssemblyName, string ClassName) GetSplitNamesFromTypename(string typename) {
            if (string.IsNullOrEmpty(typename)) {
                return ("", "");
            }

            var typeSplitString = typename.Split(char.Parse(" "));
            var typeClassName = typeSplitString[1];
            var typeAssemblyName = typeSplitString[0];
            return (typeAssemblyName, typeClassName);
        }

        public static void ShowContextMenuForManagedReference(this SerializedProperty property, bool allowNull) {
            var context = new GenericMenu();
            FillContextMenu(allowNull, context, property);
            context.ShowAsContext();
        }

        private static void FillContextMenu(bool allowNull, GenericMenu contextMenu, SerializedProperty property) {
            if (allowNull) {
                contextMenu.AddItem(new GUIContent("Null"), false, property.SetManagedReferenceToNull);
            }

            var appropriateTypes = property.GetAppropriateTypesForAssigningToManagedReference();
            foreach (var appropriateType in appropriateTypes) {
                AddItemToContextMenu(appropriateType, contextMenu, property);
            }
        }

        private static void AddItemToContextMenu(Type type, GenericMenu genericMenuContext,
            SerializedProperty property) {
            if (!type.TryGetAttribute(out SerializeReferenceMenuItemAttribute attribute)) {
                return;
            }

            var entryName = !string.IsNullOrEmpty(attribute.MenuName)
                ? attribute.MenuName
                : $"{type}  ({type.Assembly.ToString().Split('(', ',')[0]})";
            genericMenuContext.AddItem(new GUIContent(entryName), false, AssignNewInstanceCommand,
                new GenericMenuParameterForAssignInstanceCommand(type, property));
        }

        private static void AssignNewInstanceCommand(object objectGenericMenuParameter) {
            var parameter = (GenericMenuParameterForAssignInstanceCommand) objectGenericMenuParameter;
            var type = parameter.Type;
            var property = parameter.Property;
            property.AssignNewInstanceOfTypeToManagedReference(type);
        }

        private readonly struct GenericMenuParameterForAssignInstanceCommand {
            public GenericMenuParameterForAssignInstanceCommand(Type type, SerializedProperty property) {
                Type = type;
                Property = property;
            }

            public readonly SerializedProperty Property;
            public readonly Type Type;
        }
    }
}