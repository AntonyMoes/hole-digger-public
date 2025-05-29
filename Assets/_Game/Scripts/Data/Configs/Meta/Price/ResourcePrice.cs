using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Price;
using _Game.Scripts.GameAnalytics;
using _Game.Scripts.UI.Components.ResourceLike;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta.Price {
    [Serializable, SerializeReferenceMenuItem(MenuName = "Resource")]
    public class ResourcePrice : Price {
        [SerializeReferenceMenu]
        [SerializeReference] private ResourceValue.ResourceValue _value;
        
        public override bool CanPay(IPriceProcessor processor, IContainer container) {
            return processor.CanPayResource(_value);
        }

        public override bool TryPay(IPriceProcessor processor, IContainer container) {
            return processor.TryPayResource(_value);
        }

        public override IEnumerable<ResourceLikeData> GetPresentation(IContainer container) {
            return _value.Value.Select(resource => resource.ToResourceLike(true));
        }

        public override void Log(IPriceLogger logger) {
            logger.LogResource(_value);
        }
    }
}