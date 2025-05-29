using System;
using System.Linq;
using _Game.Scripts.Analytics.ByteBrew;
using _Game.Scripts.Analytics.Firebase;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.DI;

namespace _Game.Scripts.Analytics {
    public static class AnalyticsControllerFactory {
        public static AnalyticsController CreateController(AnalyticsConfig config, IContainer container) {
            var services = Enum.GetValues(typeof(AnalyticsServiceType))
                .Cast<AnalyticsServiceType>()
                .Where(type => type != AnalyticsServiceType.None && config.AnalyticServices.HasFlag(type))
                .Select(GetServiceByType);

            return container.Create<AnalyticsController>(services);
        }

        private static IAnalyticsService GetServiceByType(AnalyticsServiceType type) {
            return type switch {
                AnalyticsServiceType.Firebase => new FirebaseAnalyticsService(),
                AnalyticsServiceType.ByteBrew => new ByteBrewAnalyticsService(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}