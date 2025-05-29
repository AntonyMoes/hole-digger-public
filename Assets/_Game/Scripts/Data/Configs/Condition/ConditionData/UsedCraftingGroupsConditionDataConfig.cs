using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Condition;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Crafting;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Condition.ConditionData {
    [CreateAssetMenu(menuName = Configs.ConditionMenuItem + nameof(UsedCraftingGroupsConditionDataConfig),
        fileName = nameof(UsedCraftingGroupsConditionDataConfig))]
    public class UsedCraftingGroupsConditionDataConfig : ConditionDataConfig<bool> {
        [SerializeField] private CraftingGroupConfig[] _craftingGroups;

        protected override IConditionData<bool> PerformGetValue(IContainer container) {
            return container.Create<UsedCraftingGroupsConditionData>((IReadOnlyList<CraftingGroupConfig>) _craftingGroups);
        }

        private class UsedCraftingGroupsConditionData : ConditionData<bool> {
            private readonly IReadOnlyList<IDisposable> _subscriptionTokens;

            [Inject]
            public UsedCraftingGroupsConditionData(ICraftingController craftingController,
                IReadOnlyList<CraftingGroupConfig> craftingGroups) {
                _subscriptionTokens = craftingGroups
                    .Select(craftingController.GetCraftingGroup)
                    .SelectMany(group => group.Crafters)
                    .Select(crafter => crafter.State.Subscribe(_ => OnCrafterStateUpdated()))
                    .ToArray();
            }

            private void OnCrafterStateUpdated() {
                MutableValue.Value = true;

                foreach (var token in _subscriptionTokens) {
                    token?.Dispose();
                }
            }
        }
    }
}