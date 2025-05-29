using System;

namespace _Game.Scripts.DI {
    public interface IContainer {
        T Get<T>();
        bool TryGet<T>(out T instance);
        T Create<T>(params object[] context);
        object Create(Type type, params object[] context);
        bool TryGetOnly<T>(out T instance);
        public IContainer CreateChildContainer(Action<IContainerBuilder> callback);
    }
}