using _Game.Scripts.Data.Configs;
using _Game.Scripts.Game.Resource;

namespace _Game.Scripts.Game.Price {
    public interface IRewardProcessor {
        public bool CanAddResource(IResourceValue value);
        public bool TryAddResource(IResourceValue value);

        public bool CanAddLevel(LeveledEntityConfig config);
        public bool TryAddLevel(LeveledEntityConfig config);
    }
}