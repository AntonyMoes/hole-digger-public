using GeneralUtils.Processes;

namespace _Game.Scripts.Analytics.ByteBrew {
    public class ByteBrewAnalyticsService : IAnalyticsService {
        public Process Init() {
            return new SyncProcess(ByteBrewSDK.ByteBrew.InitializeByteBrew);
        }

        public IServiceLogger CreateLogger() {
            return new ByteBrewServiceLogger();
        }
    }
}