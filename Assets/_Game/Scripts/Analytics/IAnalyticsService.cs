using GeneralUtils.Processes;

namespace _Game.Scripts.Analytics {
    public interface IAnalyticsService {
        public abstract Process Init();
        public abstract IServiceLogger CreateLogger();
    }
}