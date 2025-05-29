using System.Linq;
using _Game.Scripts.Analytics.Events;

namespace _Game.Scripts.Analytics.ByteBrew {
    public class ByteBrewServiceLogger : IServiceLogger {
        public void Log(AnalyticsEvent analyticsEvent) {
            var parameters = analyticsEvent.GetData()
                .ToDictionary(pair => pair.Key, pair => pair.Value.ToString());

            ByteBrewSDK.ByteBrew.NewCustomEvent(analyticsEvent.Name, parameters);
        }
    }
}