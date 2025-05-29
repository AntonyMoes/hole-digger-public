using JetBrains.Annotations;

namespace _Game.Scripts.Game.Level.Digging.Tools {
    public interface IToolSettings {
        public string Name { get; }
        public string Description { get; }
    }

    public abstract class ToolSettings<TSettings> : IToolSettings where TSettings: ToolSettings<TSettings> {
        private TSettings _previousLevel;

        public abstract string Name { get; }
        public string Description => GetDescription(this, _previousLevel);

        public void SetPreviousLevel(TSettings previousLevel) {
            _previousLevel = previousLevel;
        }

        private string GetDescription(ToolSettings<TSettings> currentLevel, [CanBeNull] TSettings previousLevel) {
            return GetDescription(currentLevel as TSettings, previousLevel);
        }

        protected abstract string GetDescription(TSettings currentLevel, [CanBeNull] TSettings previousLevel);
    }
}