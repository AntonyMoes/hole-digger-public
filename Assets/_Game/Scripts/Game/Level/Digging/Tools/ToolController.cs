using System;
using System.Linq;
using _Game.Scripts.Data.Configs.Level;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Leveling;

namespace _Game.Scripts.Game.Level.Digging.Tools {
    public class ToolController : IToolController {
        private readonly Action _save;
        private readonly ITool[] _tools;

        [Inject]
        public ToolController(ILevelingController levelingController, LevelConfig levelConfig) {
            _tools = levelConfig.Tools
                .Select(config => config.GetTool(levelingController.GetLevelData(config).Level))
                .ToArray();
        }

        public ITool GetTool(ToolConfig config) {
            return _tools.First(tool => tool.Config == config);
        }

        public ITool GetTool(ICellData cell) {
            return _tools.First(tool => tool.CanMine(cell));
        }
    }
}