using System;
using _Game.Scripts.Condition;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using GeneralUtils;

namespace _Game.Scripts.Data.Configs.Condition {
    [Serializable, SerializeReferenceMenuItem(MenuName = "BoolData")]
    public class BoolDataConditionValue : ConditionWithDataValue<bool> {
        protected override ICondition PerformGetCondition(IUpdatedValue<bool> data, IContainer container) {
            return new BoolDataCondition(data);
        }

        private class BoolDataCondition : ConditionWithData<bool> {
            public BoolDataCondition(IUpdatedValue<bool> data) : base(data) {
                Data.Subscribe(value => MutableValue.Value = value, true);
            }
        }
    }
}