using GeneralUtils;

namespace _Game.Scripts.Condition {
    public interface ICondition {
        public IUpdatedValue<bool> Value { get; }
    }
}