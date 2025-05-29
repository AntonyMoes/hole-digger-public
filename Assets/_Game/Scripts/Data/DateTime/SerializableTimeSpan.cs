using System;
using UnityEngine;

namespace _Game.Scripts.Data.DateTime {
    [Serializable]
    public struct SerializableTimeSpan {
        [SerializeField] private long _ticks;

        public SerializableTimeSpan(TimeSpan dateTime) {
            _ticks = dateTime.Ticks;
        }

        public static implicit operator TimeSpan(SerializableTimeSpan dt) => new TimeSpan(dt._ticks);
    }
}