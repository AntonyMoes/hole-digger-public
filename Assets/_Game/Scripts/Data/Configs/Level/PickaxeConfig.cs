using System.Collections.Generic;
using _Game.Scripts.Game.Level.Digging.Tools;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Level {
    [CreateAssetMenu(menuName = Configs.ToolMenuItem + nameof(PickaxeConfig), fileName = nameof(PickaxeConfig))]
    public class PickaxeConfig : ToolConfig<Pickaxe.Settings> {
        protected override ITool PerformGetTool(IUpdatedValue<int> level,
            IReadOnlyList<Tool<Pickaxe.Settings>.ILevelData> levels) {
            return new Pickaxe(level, levels, this);
        }
    }
}