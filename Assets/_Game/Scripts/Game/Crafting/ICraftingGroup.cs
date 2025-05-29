using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Meta;

namespace _Game.Scripts.Game.Crafting {
    public interface ICraftingGroup {
        public IReadOnlyList<CraftingConfig> Recipes { get; }
        public IReadOnlyList<ICrafter> Crafters { get; }
    }
}