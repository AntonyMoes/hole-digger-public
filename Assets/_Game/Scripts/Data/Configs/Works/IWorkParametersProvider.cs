namespace _Game.Scripts.Data.Configs.Works {
    public interface IWorkParametersProvider {
        IWorkParameters GetParameters();
    }

    public static class WorkParametersExtensions {
        public static bool TryGetParameters<TParameters>(this IWorkParametersProvider config,
            out TParameters parameters) {
            var workParameters = config?.GetParameters();
            if (workParameters is not TParameters ps) {
                parameters = default;
                return false;
            }

            parameters = ps;
            return true;
        }
    }
}