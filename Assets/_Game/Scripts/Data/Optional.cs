using System;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.Data
{
    [Serializable]
    public struct Optional<T>
    {
        [SerializeField] private bool _enabled;
        [SerializeField] private T _value;

        public bool Enabled => _enabled;
        public T Value => _value;

        public Optional(T initialValue)
        {
            _enabled = true;
            _value = initialValue;
        }
    }

    public static class OptionalHelper
    {
        public static TStruct? ToNullable<TStruct>(this Optional<TStruct> optional) where TStruct : struct
        {
            return optional.Enabled ? optional.Value : null;
        }

        [CanBeNull]
        public static TClass ToNullableClass<TClass>(this Optional<TClass> optional) where TClass : class
        {
            return optional.Enabled ? optional.Value : null;
        }

        public static Optional<TStruct> ToOptional<TStruct>(this TStruct? value) where TStruct : struct {
            return value is {} val ? new Optional<TStruct>(val) : new Optional<TStruct>();
        }

        public static Optional<TClass> ToOptional<TClass>(this TClass obj) where TClass : class {
            return obj != null ? new Optional<TClass>(obj) : new Optional<TClass>();
        }
    }
}