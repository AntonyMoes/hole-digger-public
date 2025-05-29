using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Level;
using GeneralUtils;
using UnityEngine;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.Game.Level.Digging.Tools {
    public abstract class Tool<TSettings> : ITool where TSettings : ToolSettings<TSettings> {
        protected readonly IReadOnlyList<ILevelData> LevelData;
        protected readonly IUpdatedValue<int> Level;
        private readonly Event _levelUpdateEvent = new Event();
        public ToolConfig Config { get; }

        protected Tool(IUpdatedValue<int> level, IReadOnlyList<ILevelData> levelData,
            ToolConfig config) {
            Level = level;
            LevelData = levelData;
            Config = config;
            Level.Subscribe(_ => _levelUpdateEvent.Invoke());
        }

        public abstract bool CanMine(ICellData cell);

        public abstract IEnumerable<ICellData> Dig(Rng rng, ICellData cell,
            IReadOnlyDictionary<Vector3Int, ICellData> cells);

        public IToolView CreateView(Transform container) {
            var instance = Object.Instantiate(Config.ToolView, container);
            var view = instance.GetComponent<IToolView>();
            view.Init(this, _levelUpdateEvent, () => LevelData[Level.Value].ToolPrefab, Config.UseSounds);
            return view;
        }

        public interface ILevelData {
            public TSettings Settings { get; }
            public GameObject ToolPrefab { get; }
            public Sprite ToolIcon { get; }
        }
    }
}