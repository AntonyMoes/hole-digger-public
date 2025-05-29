using System;
using _Game.Scripts.Condition;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Resource;
using GeneralUtils;

namespace _Game.Scripts.Data.Configs.Condition {
    [Serializable, SerializeReferenceMenuItem(MenuName = "InventoryFull")]
    public class InventoryFullConditionValue : ConditionValue {
        protected override ICondition PerformGetCondition(IContainer container) {
            return container.Create<InventoryFullCondition>();
        }

        private class InventoryFullCondition : Condition {
            private readonly IResourceController _resourceController;
            private Event _cachedEvent;

            [Inject]
            public InventoryFullCondition(IResourceController resourceController) {
                _resourceController = resourceController;
                resourceController.InventorySize.Subscribe(_ => OnUpdate());
                resourceController.InventoryCapacity.Subscribe(_ => OnUpdate());
                OnUpdate();
            }

            private void OnUpdate() {
                MutableValue.Value = _resourceController.InventorySize.Value ==
                                     _resourceController.InventoryCapacity.Value;
            }
        }
    }
}