using _Game.Scripts.DI;

namespace _Game.Scripts.Data.Configs.Works {
    public abstract class WorkConfig : Config, IWorkParametersProvider {
        public abstract IWorkParameters GetParameters();

        public abstract bool Do(IContainer container);

        public abstract bool Do(IContainer container, IWorkParameters parameters);
    }

    public abstract class WorkConfig<TParameters, TFactory> : WorkConfig
        where TFactory : class, IWorkFactory<TParameters> where TParameters : IWorkParameters {
        public override IWorkParameters GetParameters() {
            return Parameters;
        }

        protected abstract TParameters Parameters { get; }

        public override bool Do(IContainer container) {
            return container.TryGet<TFactory>(out var factory) &&
                   factory.Do(Parameters);
        }

        public override bool Do(IContainer container, IWorkParameters parameters) {
            return parameters is TParameters presenterParameters
                ? container.TryGet<TFactory>(out var factory) && factory.Do(presenterParameters)
                : Do(container);
        }
    }

    public interface IWorkParameters { }

    public interface IWorkFactory<in TParameters> : IInstant where TParameters : IWorkParameters {
        bool Do(TParameters parameters);
    }

    public delegate void ActionRef<T>(ref T value);

    public static class WorkConfigExtensions {
        public static bool TryDoWithParameters<TParameters>(this WorkConfig config, IContainer container,
            ActionRef<TParameters> setParameters) {
            if (!config.TryGetParameters(out TParameters parameters) ||
                parameters is not IWorkParameters) {
                return config.Do(container);    
            }

            setParameters?.Invoke(ref parameters);
            return config.Do(container, parameters as IWorkParameters);
        }
    }
}