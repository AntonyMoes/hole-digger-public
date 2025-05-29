using System;
using _Game.Scripts.Condition;
using _Game.Scripts.DI;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Condition {
    [Serializable]
    public abstract class ConditionValue {
        [SerializeField] private bool _negate;

        public ICondition GetCondition(IContainer container) {
            var condition = PerformGetCondition(container);
            return _negate ? new NegateCondition(condition) : condition;
        }

        protected abstract ICondition PerformGetCondition(IContainer container);

        private class NegateCondition : Condition {
            public NegateCondition(ICondition condition) {
                condition.Value.Subscribe(value => MutableValue.Value = !value, true);
            }
        }
    }

    public abstract class Condition : ICondition {
        protected readonly UpdatedValue<bool> MutableValue = new UpdatedValue<bool>();
        public IUpdatedValue<bool> Value => MutableValue;
    }
}