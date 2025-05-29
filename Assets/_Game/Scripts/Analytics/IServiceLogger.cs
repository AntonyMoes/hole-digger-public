using _Game.Scripts.Analytics.Events;

namespace _Game.Scripts.Analytics {
    public interface IServiceLogger {
        public void Log(AnalyticsEvent analyticsEvent);
    }
}