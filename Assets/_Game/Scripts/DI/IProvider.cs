namespace _Game.Scripts.DI {
    public interface IProvider<T> {
        T Instance { get; }

        bool TryGet(out T instance);
    }
}