using System;
using _Game.Scripts.Data.DateTime;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Editor.DateTime {
    [CustomPropertyDrawer(typeof(SerializableDateTime))]
    public class SerializableDateTimePropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var ticks = property.FindPropertyRelative("_ticks");
            if (ticks == null) {
                return;
            }

            var oldValue = new System.DateTime(ticks.longValue, DateTimeKind.Utc);
            var newValue = DrawTimeSpan(oldValue, position, property, label);
            if (newValue.Ticks != oldValue.Ticks) {
                ticks.longValue = newValue.Ticks;
            }
        }

        private static System.DateTime DrawTimeSpan(System.DateTime current, Rect position, SerializedProperty property,
            GUIContent label) {
            var year = (int?) current.Year;
            var month = (int?) current.Month;
            var day = current.Day;
            var hour = current.Hour;
            var minute = current.Minute;
            var second = current.Second;

            PropertyDrawerHelper.Draw(position, property, label, ref year, ref month, ref day, ref hour, ref minute,
                ref second);

            return new System.DateTime(year!.Value, month!.Value, day, hour, minute, second, DateTimeKind.Utc);
        }
    }
}