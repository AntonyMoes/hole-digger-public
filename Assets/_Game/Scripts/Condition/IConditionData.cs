using GeneralUtils;

namespace _Game.Scripts.Condition {
    public interface IConditionData {
        public void Init(IConditionDataStorage storage);
    }

    public interface IConditionData<T> : IConditionData {
        public IUpdatedValue<T> Data { get; }
    }
}