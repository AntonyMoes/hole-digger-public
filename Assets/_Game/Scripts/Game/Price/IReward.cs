using System.Collections.Generic;
using _Game.Scripts.DI;
using _Game.Scripts.GameAnalytics;
using _Game.Scripts.UI.Components.ResourceLike;

namespace _Game.Scripts.Game.Price {
    public interface IReward {
        public bool CanAdd(IRewardProcessor processor, IContainer container);
        public bool TryAdd(IRewardProcessor processor, IContainer container);

        public IEnumerable<ResourceLikeData> GetPresentation(IContainer container);
        public void Log(IRewardLogger logger);
    }
}