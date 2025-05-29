using System;
using System.Collections.Generic;
using System.Linq;
using GeneralUtils;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Level {
    [CreateAssetMenu(menuName = Configs.LevelMenuItem + nameof(OreGenerationConfig), fileName = nameof(OreGenerationConfig))]
    public class OreGenerationConfig : Config {
        [SerializeField] private float _baseChance;

        [ArrayElementTitle("oreConfig")]
        [SerializeField] private Ore[] _values;

        public IEnumerable<OreConfig> Ores => _values.Select(ore => ore.oreConfig);

        [CanBeNull]
        public OreConfig GenerateOre(Rng rng) {
            if (!rng.NextProbabilityCheck(_baseChance)) {
                return null;
            }

            return rng.NextWeightedChoice(_values.Select(ore => (ore.oreConfig, ore.weight)).ToArray());
        }

        [Serializable]
        private class Ore {
            public OreConfig oreConfig;
            public float weight;
        }
    }
}