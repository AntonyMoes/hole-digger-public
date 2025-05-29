using _Game.Scripts.Analytics.Events;

namespace _Game.Scripts.GameAnalytics.Events {
    public class LevelEvent : AnalyticsEvent {
        [LogField] private int level;
        [LogField] private string name;

        public LevelEvent(int level, string name) : base("level") {
            this.level = level;
            this.name = name;
        }
    }
}