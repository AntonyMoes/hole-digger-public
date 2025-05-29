using System;
using System.Collections.Generic;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Price;
using _Game.Scripts.GameAnalytics;
using _Game.Scripts.UI.Components.ResourceLike;

namespace _Game.Scripts.Data.Configs.Meta.Reward {
    [Serializable]
    public abstract class Reward : IReward {
        public abstract bool CanAdd(IRewardProcessor processor, IContainer container);
        public abstract bool TryAdd(IRewardProcessor processor, IContainer container);
        public abstract IEnumerable<ResourceLikeData> GetPresentation(IContainer container);
        public abstract void Log(IRewardLogger logger);
    }
}