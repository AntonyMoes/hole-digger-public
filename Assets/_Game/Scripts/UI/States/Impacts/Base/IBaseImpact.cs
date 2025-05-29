namespace _Game.Scripts.UI.States.Impacts.Base {
    public interface IBaseImpact<in T> : IBaseImpact where T : UnityEngine.Object {
        void Apply(T target);

        void FillDefaultValues(T target);
    }

    public interface IBaseImpact { }
}