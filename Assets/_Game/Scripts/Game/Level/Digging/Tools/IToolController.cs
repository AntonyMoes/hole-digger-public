using _Game.Scripts.Data.Configs.Level;

namespace _Game.Scripts.Game.Level.Digging.Tools {
    public interface IToolController {
        public ITool GetTool(ToolConfig config);
        public ITool GetTool(ICellData cell);
    }
}