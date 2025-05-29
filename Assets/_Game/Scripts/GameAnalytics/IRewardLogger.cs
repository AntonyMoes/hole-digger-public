using _Game.Scripts.Data.Configs;
using _Game.Scripts.Game.Resource;

namespace _Game.Scripts.GameAnalytics {
    public interface IRewardLogger {
        void LogResource(IResourceValue value);
        void LogLevel(LeveledEntityConfig config);
    }
}