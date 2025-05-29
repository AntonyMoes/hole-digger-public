using _Game.Scripts.Analytics.Events;
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
using System.Linq;
using Firebase.Analytics;
#endif

namespace _Game.Scripts.Analytics.Firebase {
    public class FirebaseServiceLogger : IServiceLogger {
        public void Log(AnalyticsEvent analyticsEvent) {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            var parameters = analyticsEvent.GetData()
                .Select(pair => pair.Value is int intValue
                    ? new Parameter(pair.Key, intValue)
                    : new Parameter(pair.Key, pair.Value.ToString()))
                .ToArray();
            FirebaseAnalytics.LogEvent(analyticsEvent.Name, parameters);
#endif
        }
    }
}