using System;
using System.Collections.Generic;
using UnityEditor;

namespace _Game.Scripts.Editor.SerializedReference {
    public interface IMenuItemFilter {
        Type ParentType { get; }

        IEnumerable<Type> Handle(SerializedProperty property, IEnumerable<Type> derivedTypes);
    }
}