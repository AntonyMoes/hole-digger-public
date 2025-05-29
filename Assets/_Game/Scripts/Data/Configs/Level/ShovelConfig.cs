using System.Collections.Generic;
using _Game.Scripts.Game.Level.Digging.Tools;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Level {
    [CreateAssetMenu(menuName = Configs.ToolMenuItem + nameof(ShovelConfig), fileName = nameof(ShovelConfig))]
    public class ShovelConfig : ToolConfig<Shovel.Settings> {
        protected override ITool PerformGetTool(IUpdatedValue<int> level,
            IReadOnlyList<Tool<Shovel.Settings>.ILevelData> levels) {
            return new Shovel(level, levels, this);
        }
    }
}