using GeneralUtils;

namespace _Game.Scripts.Game.Leveling {
    public interface ILevelingData {
        public IUpdatedValue<int> Level { get; }
        public int MaxLevel { get; }
    }
}