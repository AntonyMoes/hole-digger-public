using System;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Game.Resource;
using GeneralUtils;

namespace _Game.Scripts.Game.Crafting {
    public interface IReadOnlyCrafter {
        public IUpdatedValue<CrafterState> State { get; }

        public IUpdatedValue<CraftingConfig> CurrentProcess { get; }
        public int Amount { get; }

        public IUpdatedValue<TimeSpan?> TimeToCompletion { get; }

        public bool CanStartCrafting(CraftingConfig config, int amount, out CantCraftReason reason);
        public bool CanCollectResult();
    }

    public interface ICrafter : IReadOnlyCrafter {
        public bool TryStartCrafting(CraftingConfig config, int amount, out CantCraftReason reason);
        public bool TryCollectResult();
    }

    public static class CrafterHelper {
        public static IResourceValue Reward(this CraftingConfig config, int amount) {
            return config.Reward.Multiply(amount);
        }

        public static IResourceValue Price(this CraftingConfig config, int amount) {
            return config.Price.Multiply(amount);
        }

        public static TimeSpan Time(this CraftingConfig config, int amount) {
            return config.Time * amount;
        }

        public static IResourceValue Reward(this IReadOnlyCrafter crafter) {
            return crafter.CurrentProcess.Value.Reward(crafter.Amount);
        }

        public static IResourceValue Price(this IReadOnlyCrafter crafter) {
            return crafter.CurrentProcess.Value.Price(crafter.Amount);
        }

        public static TimeSpan Time(this IReadOnlyCrafter crafter) {
            return crafter.CurrentProcess.Value.Time(crafter.Amount);
        }
    }
}