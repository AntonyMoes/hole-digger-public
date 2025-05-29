using _Game.Scripts.Data.Configs.Meta;

namespace _Game.Scripts.Game.Crafting {
    public interface ICraftingController {
        public ICraftingGroup GetCraftingGroup(CraftingGroupConfig config);
    }
}