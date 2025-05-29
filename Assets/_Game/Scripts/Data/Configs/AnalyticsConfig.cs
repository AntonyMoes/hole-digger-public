using _Game.Scripts.Analytics;
using _Game.Scripts.Data.Configs.Meta;
using UnityEngine;

namespace _Game.Scripts.Data.Configs {
    [CreateAssetMenu(menuName = Configs.MenuItem + nameof(AnalyticsConfig), fileName = nameof(AnalyticsConfig))]
    public class AnalyticsConfig : Config {
        [SerializeField] private AnalyticsServiceType _analyticServices;
        public AnalyticsServiceType AnalyticServices => _analyticServices;

        [SerializeField] private int _levelLoggingFrequency;
        public int LevelLoggingFrequency => _levelLoggingFrequency;

        [SerializeField] private CraftingGroupConfig _loggedCraftingGroupConfig;
        public CraftingGroupConfig LoggedCraftingGroupConfig => _loggedCraftingGroupConfig;
    }
}