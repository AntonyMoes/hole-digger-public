using _Game.Scripts.Game.Resource;

namespace _Game.Scripts.GameAnalytics {
    public interface IPriceLogger {
        public void LogResource(IResourceValue value);
    }
}