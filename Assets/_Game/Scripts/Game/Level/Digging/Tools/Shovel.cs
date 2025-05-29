using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data.Configs.Level;
using _Game.Scripts.DI;
using _Game.Scripts.Utils;
using GeneralUtils;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging.Tools {
    public class Shovel : Tool<Shovel.Settings> {
        [Inject]
        public Shovel(IUpdatedValue<int> level, IReadOnlyList<ILevelData> levelData, ToolConfig config) : base(level,
            levelData, config) { }

        public override bool CanMine(ICellData cell) {
            return cell.OreConfig == null;
        }

        public override IEnumerable<ICellData> Dig(Rng rng, ICellData cell,
            IReadOnlyDictionary<Vector3Int, ICellData> cells) {
            var settings = LevelData[Level.Value].Settings;

            cell.StrikesRemaining -= 1;
            if (cell.StrikesRemaining == 0 && rng.NextProbabilityCheck(settings.MineAdjacentChance) &&
                TryFindAdjacent(rng, cell, cells) is { } adjacent) {
                adjacent.StrikesRemaining = 0;
                return Enumerable.Empty<ICellData>()
                    .Append(cell)
                    .Append(adjacent);
            }

            return Enumerable.Empty<ICellData>().Append(cell);
        }

        [CanBeNull] private ICellData TryFindAdjacent(Rng rng, ICellData cell,
            IReadOnlyDictionary<Vector3Int, ICellData> cells) {
            var checkDirections = new[] {
                Vector3Int.left,
                Vector3Int.right,
                Vector3Int.up,
                Vector3Int.down,
            };

            foreach (var direction in rng.NextShuffle(checkDirections)) {
                if (cells.TryGetValue(cell.Index + direction, out var adjacent) && CanMine(adjacent)) {
                    return adjacent;
                }
            }

            return null;
        }

        [Serializable]
        public class Settings : ToolSettings<Settings> {
            [Range(0, 1)] [SerializeField] private float _mineAdjacentChance;
            public float MineAdjacentChance => _mineAdjacentChance;

            public override string Name => "Shovel";

            protected override string GetDescription(Settings currentLevel, Settings previousLevel) {
                if (previousLevel == null) {
                    return "";
                }

                return "Increases the chance to dig an adjacent tile from " +
                       $"{previousLevel.MineAdjacentChance.FormatProbability()} to " +
                       $"{currentLevel.MineAdjacentChance.FormatProbability()}";
            }
        }
    }
}