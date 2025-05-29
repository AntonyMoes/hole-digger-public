using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Leveling;
using _Game.Scripts.Game.Price;
using _Game.Scripts.GameAnalytics;
using _Game.Scripts.UI.Components.ResourceLike;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta.Reward {
    [Serializable, SerializeReferenceMenuItem(MenuName = "Level")]
    public class LevelReward : Reward {
        [SerializeField] private LeveledEntityConfig _leveledEntityConfig;

        public override bool CanAdd(IRewardProcessor processor, IContainer container) {
            return processor.CanAddLevel(_leveledEntityConfig);
        }

        public override bool TryAdd(IRewardProcessor processor, IContainer container) {
            return processor.TryAddLevel(_leveledEntityConfig);
        }

        public override IEnumerable<ResourceLikeData> GetPresentation(IContainer container) {
            var levelData = container.Get<ILevelingController>().GetLevelData(_leveledEntityConfig);
            return Enumerable.Empty<ResourceLikeData>()
                .Append(_leveledEntityConfig.ToResourceLike(container, levelData.Level.Value + 1));
        }

        public override void Log(IRewardLogger logger) {
            logger.LogLevel(_leveledEntityConfig);
        }

        public LevelReward(LeveledEntityConfig config) {
            _leveledEntityConfig = config;
        }
    }
}