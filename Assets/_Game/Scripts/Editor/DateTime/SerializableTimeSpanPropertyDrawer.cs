using System;
using _Game.Scripts.Data.DateTime;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Editor.DateTime {
    [CustomPropertyDrawer(typeof(SerializableTimeSpan))]
    public class SerializableTimeSpanPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var ticks = property.FindPropertyRelative("_ticks");
            if (ticks == null) {
                return;
            }

            var oldValue = new TimeSpan(ticks.longValue);
            var newValue = DrawTimeSpan(oldValue, position, property, label);
            if (newValue.Ticks != oldValue.Ticks) {
                ticks.longValue = newValue.Ticks;
            }
        }

        private static TimeSpan DrawTimeSpan(TimeSpan current, Rect position, SerializedProperty property,
            GUIContent label) {
            var year = (int?) null;
            var month = (int?) null;
            var day = current.Days;
            var hour = current.Hours;
            var minute = current.Minutes;
            var second = current.Seconds;

            PropertyDrawerHelper.Draw(position, property, label, ref year, ref month, ref day, ref hour, ref minute,
                ref second);

            return new TimeSpan(day, hour, minute, second);
        }
    }
}