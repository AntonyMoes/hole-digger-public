using System;
using System.Collections.Generic;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Leveling;
using _Game.Scripts.Game.Price;
using _Game.Scripts.Utils;
using UnityEngine;

namespace _Game.Scripts.UI.Components.ResourceLike {
    public struct ResourceLikeData {
        public string Name;
        public string Amount;
        public string Description;
        public Sprite Icon;
    }

    public struct TransactionResourceLikeData {
        public IEnumerable<ResourceLikeData> PriceData;
        public IEnumerable<ResourceLikeData> RewardData;
    }

    public static class ResourceLikeDataHelper {
        public static ResourceLikeData ToResourceLike(this Game.Resource.Resource resource, bool abs = false,
            bool addPlus = false) {
            return new ResourceLikeData {
                Name = resource.Config.Name,
                Amount = (addPlus ? "+" : "") + (abs ? Math.Abs(resource.Amount) : resource.Amount),
                Icon = resource.Config.Sprite
            };
        }

        public static ResourceLikeData ToResourceLike(this TimeSpan time, UIConfig uiConfig) {
            return new ResourceLikeData {
                Amount = time.FormatTimer(),
                Icon = uiConfig.TimeIcon
            };
        }

        public static ResourceLikeData ToResourceLike(this LeveledEntityConfig config, IContainer container,
            int? levelOverride = null) {
            var level = levelOverride ?? container.Get<ILevelingController>().GetLevelData(config).Level.Value;
            var levelInfo = config.LevelsInfo[level];
            return new ResourceLikeData {
                Name = levelInfo.Name,
                Amount = "Level " + level,
                Description = levelInfo.Description,
                Icon = levelInfo.Icon
            };
        }

        public static TransactionResourceLikeData ToResourceLike(this Transaction transaction, IContainer container) {
            return new TransactionResourceLikeData {
                PriceData = transaction.Price.GetPresentation(container),
                RewardData = transaction.Reward.GetPresentation(container)
            };
        }
    }
}