using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Condition.ConditionData;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Condition {
    [CreateAssetMenu(menuName = Configs.ConditionMenuItem + nameof(ConditionsConfig),
        fileName = nameof(ConditionsConfig))]
    public class ConditionsConfig : Config {
        [SerializeField] private ConditionDataConfig[] _trackedDataConfigs;
        public IReadOnlyList<ConditionDataConfig> TrackedDataConfigs => _trackedDataConfigs;
    }
}