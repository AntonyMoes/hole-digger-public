using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data.Configs.Level;
using _Game.Scripts.DI;
using _Game.Scripts.Utils;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging.Tools {
    public class Pickaxe : Tool<Pickaxe.Settings> {
        [Inject]
        public Pickaxe(IUpdatedValue<int> level, IReadOnlyList<ILevelData> levelData, ToolConfig config) : base(level,
            levelData, config) { }

        public override bool CanMine(ICellData cell) {
            return cell.OreConfig != null;
        }

        public override IEnumerable<ICellData> Dig(Rng rng, ICellData cell,
            IReadOnlyDictionary<Vector3Int, ICellData> cells) {
            var settings = LevelData[Level.Value].Settings;

            cell.StrikesRemaining -= 1;
            if (cell.StrikesRemaining == 0 && rng.NextProbabilityCheck(settings.DoubleDropChance)) {
                cell.DropCount *= 2;
            }

            return Enumerable.Empty<ICellData>().Append(cell);
        }

        [Serializable]
        public class Settings : ToolSettings<Settings> {
            [Range(0, 1)] [SerializeField] private float _doubleDropChance;
            public float DoubleDropChance => _doubleDropChance;

            public override string Name => "Pickaxe";

            protected override string GetDescription(Settings currentLevel, Settings previousLevel) {
                if (previousLevel == null) {
                    return "";
                }

                return "Increases the chance to get double drops from " +
                       $"{previousLevel.DoubleDropChance.FormatProbability()} to " +
                       $"{currentLevel.DoubleDropChance.FormatProbability()}";
            }
        }
    }
}