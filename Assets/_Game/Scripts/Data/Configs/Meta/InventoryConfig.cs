using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta {
    [CreateAssetMenu(menuName = Configs.MetaMenuItem + nameof(InventoryConfig), fileName = nameof(InventoryConfig))]
    public class InventoryConfig : LeveledEntityConfig {
        [SerializeField] private Sprite _icon;
        [SerializeField] private int[] _sizeLevels;
        public IReadOnlyList<int> SizeLevels => _sizeLevels;

        public override IReadOnlyList<LevelInfo> LevelsInfo => _sizeLevels.Select((size, i) => new LevelInfo {
            Name = "Inventory",
            Description = i == 0 ? "" : $"Increases the inventory size\n from {_sizeLevels[i - 1]} to {size}",
            Icon = _icon
        }).ToArray();
    }
}