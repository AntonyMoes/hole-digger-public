namespace _Game.Scripts.DI {
    public interface IContainerBuilder {
        void AddInstance<T>(T instance, params object[] context);
        TInstance CreateInstance<T, TInstance>(params object[] context) where TInstance : T;
        void AddType<T, TInstance>() where TInstance : T;
    }
}