namespace _Game.Scripts.Condition {
    public interface IConditionDataStorage {
        public TData Load<TData>();
        public void Save<TData>(TData data);
    }
}