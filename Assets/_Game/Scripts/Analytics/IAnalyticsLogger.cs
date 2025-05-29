using _Game.Scripts.Analytics.Events;

namespace _Game.Scripts.Analytics {
    public interface IAnalyticsLogger {
        public void Log(AnalyticsEvent analyticsEvent);
    }
}