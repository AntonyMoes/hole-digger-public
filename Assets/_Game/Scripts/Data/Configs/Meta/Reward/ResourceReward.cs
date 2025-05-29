using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Price;
using _Game.Scripts.GameAnalytics;
using _Game.Scripts.UI.Components.ResourceLike;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta.Reward {
    [Serializable, SerializeReferenceMenuItem(MenuName = "Resource")]
    public class ResourceReward : Reward {
        [SerializeReferenceMenu]
        [SerializeReference] private ResourceValue.ResourceValue _value;

        public override bool CanAdd(IRewardProcessor processor, IContainer container) {
            return processor.CanAddResource(_value);
        }

        public override bool TryAdd(IRewardProcessor processor, IContainer container) {
            return processor.TryAddResource(_value);
        }

        public override IEnumerable<ResourceLikeData> GetPresentation(IContainer container) {
            return _value.Value.Select(resource => resource.ToResourceLike());
        }

        public override void Log(IRewardLogger logger) {
            logger.LogResource(_value);
        }
    }
}