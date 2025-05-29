using System.Collections.Generic;
using _Game.Scripts.Game.Resource;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Game.Level {
    public interface ILevelController {
        public UpdatedValue<bool> PrioritizeResources { get; }
        
        public UpdatedValue<int> Depth { get; }
        public UpdatedValue<IReadOnlyList<LevelCell>> LevelCells { get; }
        public IUpdatedValue<Dictionary<Vector3Int, LevelCell>> LevelCellsDictionary { get; }

        public IEvent<IResourceValue> CollectDropEvent { get; }
        public bool TryCollectDrop(IResourceValue value, out CantAddReason reason);

        public class LevelCell {
            public Vector3Int Index;
            public int? OreConfigId;
        }
    }
}