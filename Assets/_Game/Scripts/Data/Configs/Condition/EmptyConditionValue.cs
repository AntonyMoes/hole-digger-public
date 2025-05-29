using System;
using _Game.Scripts.Condition;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;

namespace _Game.Scripts.Data.Configs.Condition {
    [Serializable, SerializeReferenceMenuItem(MenuName = "Empty")]
    public class EmptyConditionValue : ConditionValue {
        protected override ICondition PerformGetCondition(IContainer container) {
            return new EmptyCondition();
        }

        private class EmptyCondition : Condition {
            public EmptyCondition() {
                MutableValue.Value = true;
            }
        }
    }
}