using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Data.Configs {
    public abstract class LeveledEntityConfig : Config {
        [SerializeField] private string _logName;
        public string LogName => _logName;

        public abstract IReadOnlyList<LevelInfo> LevelsInfo { get; }

        public struct LevelInfo {
            public string Name;
            public string Description;
            public Sprite Icon;
        }
    }
}