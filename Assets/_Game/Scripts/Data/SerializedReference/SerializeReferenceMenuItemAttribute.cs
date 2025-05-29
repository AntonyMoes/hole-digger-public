using System;

namespace _Game.Scripts.Data.SerializedReference {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class SerializeReferenceMenuItemAttribute : Attribute {
        public string MenuName;
    }
}