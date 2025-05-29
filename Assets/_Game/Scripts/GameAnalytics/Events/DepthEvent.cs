using _Game.Scripts.Analytics.Events;

namespace _Game.Scripts.GameAnalytics.Events {
    public class DepthEvent : AnalyticsEvent {
        [LogField] private int depth;

        public DepthEvent(int depth) : base("depth") {
            this.depth = depth;
        }
    }
}