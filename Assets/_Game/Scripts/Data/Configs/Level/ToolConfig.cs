using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Game.Level.Digging.Tools;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Level {
    public abstract class ToolConfig : LeveledEntityConfig {
        [SerializeField] private GameObject _toolView;
        public GameObject ToolView => _toolView;

        [SerializeField] private SoundConfig[] _useSounds;
        public IEnumerable<SoundConfig> UseSounds => _useSounds;

        public abstract ITool GetTool(IUpdatedValue<int> level);
    }

    public abstract class ToolConfig<TSettings> : ToolConfig where TSettings : ToolSettings<TSettings> {
        [SerializeField] private LevelData[] _levels;
        private bool _initSettings;

        public sealed override ITool GetTool(IUpdatedValue<int> level) {
            return PerformGetTool(level, _levels);
        }

        protected abstract ITool PerformGetTool(IUpdatedValue<int> level,
            IReadOnlyList<Tool<TSettings>.ILevelData> levels);

        public override IReadOnlyList<LevelInfo> LevelsInfo {
            get {
                if (!_initSettings) {
                    _initSettings = true;
                    for (var i = 1; i < _levels.Length; i++) {
                        _levels[i].Settings.SetPreviousLevel(_levels[i - 1].Settings);
                    }
                }

                return _levels.Select(level => new LevelInfo {
                    Name = level.Settings.Name,
                    Description = level.Settings.Description,
                    Icon = level.ToolIcon
                }).ToArray();
            }
        }

        [Serializable]
        private class LevelData : Tool<TSettings>.ILevelData {
            [SerializeField] private TSettings _settings;
            public TSettings Settings => _settings;

            [SerializeField] private GameObject _toolPrefab;
            public GameObject ToolPrefab => _toolPrefab;

            [SerializeField] private Sprite _toolIcon;
            public Sprite ToolIcon => _toolIcon;
        }
    }
}