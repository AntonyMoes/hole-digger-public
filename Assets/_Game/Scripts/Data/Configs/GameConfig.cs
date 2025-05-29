using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Condition;
using _Game.Scripts.Data.Configs.Level;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Data.Configs.Tutorial;
using _Game.Scripts.Data.Configs.Works;
using UnityEngine;

namespace _Game.Scripts.Data.Configs {
    [CreateAssetMenu(menuName = Configs.MenuItem + nameof(GameConfig), fileName = nameof(GameConfig))]
    public class GameConfig : Config {
        [Header("Entry")]
        [SerializeField] private WorkConfig _loadingWork;
        public WorkConfig LoadingWork => _loadingWork;

        [SerializeField] private WorkConfig _entryWork;
        public WorkConfig EntryWork => _entryWork;

        [Header("Level")]
        [SerializeField] private LevelConfig _levelConfig;
        public LevelConfig LevelConfig => _levelConfig;

        [Header("Resources")]
        [SerializeField] private ResourceConfig[] _resources;
        public IEnumerable<ResourceConfig> Resources => _resources;

        [SerializeField] private InventoryConfig _inventoryConfig;
        public InventoryConfig InventoryConfig => _inventoryConfig;

        [Header("UI")]
        [SerializeField] private UIConfig _uiConfig;
        public UIConfig UIConfig => _uiConfig;
        
        [Header("Conditions")]
        [SerializeField] private ConditionsConfig _conditionsConfig;
        public ConditionsConfig ConditionsConfig => _conditionsConfig;
        
        [Header("Analytics")]
        [SerializeField] private AnalyticsConfig _analyticsConfig;
        public AnalyticsConfig AnalyticsConfig => _analyticsConfig;
        
        [Header("Tutorials")]
        [SerializeField] private TutorialsConfig _tutorialsConfig;
        public TutorialsConfig TutorialsConfig => _tutorialsConfig;
    }
}