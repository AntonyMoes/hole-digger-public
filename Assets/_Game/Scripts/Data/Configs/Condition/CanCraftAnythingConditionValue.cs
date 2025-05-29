using System;
using System.Linq;
using _Game.Scripts.Condition;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Crafting;
using _Game.Scripts.Game.Resource;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Condition {
    [Serializable, SerializeReferenceMenuItem(MenuName = "CanCraftAnything")]
    public class CanCraftAnythingConditionValue : ConditionValue {
        [SerializeField] private CraftingGroupConfig _craftingGroup;

        protected override ICondition PerformGetCondition(IContainer container) {
            return container.Create<CanCraftAnythingCondition>(_craftingGroup);
        }

        private class CanCraftAnythingCondition : Condition {
            private readonly IResourceController _resourceController;
            private readonly ICraftingGroup _group;

            [Inject]
            public CanCraftAnythingCondition(ICraftingController craftingController,
                IResourceController resourceController, CraftingGroupConfig craftingGroup) {
                _resourceController = resourceController;
                _group = craftingController.GetCraftingGroup(craftingGroup);

                var recipeResources = _group.Recipes
                    .SelectMany(recipe => recipe.Price.Value)
                    .Select(resource => resource.Config)
                    .ToHashSet();

                foreach (var resource in recipeResources) {
                    resourceController.GetResource(resource).Amount.Subscribe(_ => OnResourceUpdate());
                }

                OnResourceUpdate();
            }

            private void OnResourceUpdate() {
                MutableValue.Value = _group.Recipes.Any(recipe => _resourceController.CanAdd(recipe.Price));
            }
        }
    }
}