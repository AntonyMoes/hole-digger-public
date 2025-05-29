using System;

namespace _Game.Scripts.Utils {
    public static class FormatExtensions {
        public static string FormatTimer(this TimeSpan timeSpan) {
            if (timeSpan.TotalHours < 1) {
                return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            }

            var hours = Convert.ToInt32(Math.Floor(timeSpan.TotalHours));
            return $"{hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }

        public static string FormatProbability(this float probability) {
            return $"{probability * 100}%";
        }
    }
}