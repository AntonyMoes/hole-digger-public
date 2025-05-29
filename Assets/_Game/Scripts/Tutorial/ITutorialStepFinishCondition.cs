using _Game.Scripts.DI;
using GeneralUtils;

namespace _Game.Scripts.Tutorial {
    public interface ITutorialStepFinishCondition {
        public IEvent FinishEvent { get; }
        public void Init(Parameters parameters);

        public struct Parameters {
            public IContainer Container { get; set; }
            public TutorialHider Hider { get; set; }
            public Event<string> InitErrorEvent { get; set; }
        }
    }
}