using _Game.Scripts.Analytics.Events;

namespace _Game.Scripts.GameAnalytics.Events {
    public class TutorialEvent : AnalyticsEvent {
        [LogField] private string tutorial;
        [LogField] private string state;

        public TutorialEvent(string tutorial, TutorialState state) : base("tutorial") {
            this.tutorial = tutorial;
            this.state = state.ToString().ToLower();
        }

        public enum TutorialState {
            Start,
            Complete
        }
    }
}