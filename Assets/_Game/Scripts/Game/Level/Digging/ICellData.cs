using _Game.Scripts.Data.Configs.Level;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging {
    public interface ICellData {
        public Vector3Int Index { get; }
        public int StrikesRemaining { get; set; }
        public int DropCount { get; set; }
        [CanBeNull] public OreConfig OreConfig { get;}
    }
}