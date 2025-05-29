using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta {
    [CreateAssetMenu(menuName = Configs.MetaMenuItem + nameof(CraftingGroupConfig), fileName = nameof(CraftingGroupConfig))]
    public class CraftingGroupConfig : Config {
        [SerializeField] private CraftingConfig[] _recipes;
        public CraftingConfig[] Recipes => _recipes;

        [SerializeField] private int _crafterCount;
        public int CrafterCount => _crafterCount;
    }
}