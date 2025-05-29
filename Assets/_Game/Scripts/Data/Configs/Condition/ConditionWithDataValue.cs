using System;
using _Game.Scripts.Condition;
using _Game.Scripts.Data.Configs.Condition.ConditionData;
using _Game.Scripts.DI;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Condition {
    [Serializable]
    public abstract class ConditionWithDataValue<TData> : ConditionValue {
        [SerializeField] private ConditionDataConfig<TData> _dataConfig;
        
        protected sealed override ICondition PerformGetCondition(IContainer container) {
            var data = container.Get<IConditionDataController>().GetConditionData<TData>(_dataConfig.ConfigId);
            return PerformGetCondition(data, container);
        }

        protected abstract ICondition PerformGetCondition(IUpdatedValue<TData> data, IContainer container);
    }

    public abstract class ConditionWithData<TData> : Condition {
        protected readonly IUpdatedValue<TData> Data;

        protected ConditionWithData(IUpdatedValue<TData> data) {
            Data = data;
        }
    }
}