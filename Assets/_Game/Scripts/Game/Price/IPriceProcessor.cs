using _Game.Scripts.Game.Resource;

namespace _Game.Scripts.Game.Price {
    public interface IPriceProcessor {
        public bool CanPayResource(IResourceValue value);
        public bool TryPayResource(IResourceValue value);
    }
}