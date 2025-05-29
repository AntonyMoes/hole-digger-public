using _Game.Scripts.Condition;
using _Game.Scripts.DI;
using GeneralUtils;

namespace _Game.Scripts.Data.Configs.Condition.ConditionData {
    
    public abstract class ConditionDataConfig: Config {
        public abstract IConditionData GetValue(IContainer container);
    }
    public abstract class ConditionDataConfig<TData> : ConditionDataConfig {
        public override IConditionData GetValue(IContainer container) => PerformGetValue(container);
        protected abstract IConditionData<TData> PerformGetValue(IContainer container);
    }

    public class ConditionData<TData> : IConditionData<TData> {
        protected readonly UpdatedValue<TData> MutableValue = new UpdatedValue<TData>();
        public IUpdatedValue<TData> Data => MutableValue;

        public void Init(IConditionDataStorage storage) {
            MutableValue.Value = storage.Load<TData>();
            MutableValue.Subscribe(storage.Save);
        }
    }
}