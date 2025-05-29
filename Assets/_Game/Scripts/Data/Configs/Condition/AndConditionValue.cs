using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Condition;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Condition {
    [Serializable, SerializeReferenceMenuItem(MenuName = "And")]
    public class AndConditionValue : ConditionValue {
        [SerializeReferenceMenu]
        [SerializeReference] private ConditionValue[] _values;

        protected override ICondition PerformGetCondition(IContainer container) {
            return container.Create<AndCondition>((IEnumerable<ConditionValue>) _values);
        }

        private class AndCondition : Condition {
            private readonly IReadOnlyList<ICondition> _conditions;

            [Inject]
            public AndCondition(IEnumerable<ConditionValue> values, IContainer container) {
                _conditions = values.Select(value => {
                    var condition = value.GetCondition(container);
                    condition.Value.Subscribe(_ => OnUpdate());
                    return condition;
                }).ToArray();
                OnUpdate();
            }

            private void OnUpdate() {
                MutableValue.Value = _conditions.All(condition => condition.Value.Value);
            }
        }
    }
}