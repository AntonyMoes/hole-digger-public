using System;
using System.Collections.Generic;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Price;
using _Game.Scripts.GameAnalytics;
using _Game.Scripts.UI.Components.ResourceLike;

namespace _Game.Scripts.Data.Configs.Meta.Price {
    [Serializable]
    public abstract class Price : IPrice {
        public abstract bool CanPay(IPriceProcessor processor, IContainer container);
        public abstract bool TryPay(IPriceProcessor processor, IContainer container);
        public abstract IEnumerable<ResourceLikeData> GetPresentation(IContainer container);
        public abstract void Log(IPriceLogger logger);
    }
}