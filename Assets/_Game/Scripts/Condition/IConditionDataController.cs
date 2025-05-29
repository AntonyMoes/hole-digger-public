using GeneralUtils;

namespace _Game.Scripts.Condition {
    public interface IConditionDataController {
        public IUpdatedValue<TData> GetConditionData<TData>(int configId);
    }
}