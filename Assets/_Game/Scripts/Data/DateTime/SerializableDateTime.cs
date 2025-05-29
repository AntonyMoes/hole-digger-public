using System;
using UnityEngine;

namespace _Game.Scripts.Data.DateTime {
    [Serializable]
    public struct SerializableDateTime {
        [SerializeField] private long _ticks;

        public SerializableDateTime(System.DateTime dateTime) {
            _ticks = dateTime.Ticks;
        }

        public static implicit operator System.DateTime(SerializableDateTime dt) =>
            new System.DateTime(dt._ticks, DateTimeKind.Utc);

        public static implicit operator SerializableDateTime(System.DateTime dt) => new SerializableDateTime(dt);
    }
}