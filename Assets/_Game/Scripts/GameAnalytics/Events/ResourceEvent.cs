using _Game.Scripts.Analytics.Events;

namespace _Game.Scripts.GameAnalytics.Events {
    public class ResourceEvent : AnalyticsEvent {
        public const string CraftReason = "craft";
        public const string DropReason = "drop";

        [LogField] private int delta;
        [LogField] private string resource;
        [LogField] private string reason;

        public ResourceEvent(int delta, string resource, string reason) : base("resource") {
            this.delta = delta;
            this.resource = resource;
            this.reason = reason;
        }
    }
}