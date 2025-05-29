using _Game.Scripts.Data.Configs;

namespace _Game.Scripts.Game.Leveling {
    public interface ILevelingController {
        public ILevelingData GetLevelData(LeveledEntityConfig config);
        public bool CanAddLevel(LeveledEntityConfig config);
        public bool TryAddLevel(LeveledEntityConfig config);
    }
}