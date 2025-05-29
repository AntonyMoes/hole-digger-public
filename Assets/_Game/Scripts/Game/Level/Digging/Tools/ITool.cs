using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Level;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging.Tools {
    public interface ITool {
        public ToolConfig Config { get; }

        bool CanMine(ICellData cell);
        public IEnumerable<ICellData> Dig(Rng rng, ICellData cell, IReadOnlyDictionary<Vector3Int, ICellData> cells);
        public IToolView CreateView(Transform container);
    }
}