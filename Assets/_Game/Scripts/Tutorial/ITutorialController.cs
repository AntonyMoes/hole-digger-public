using _Game.Scripts.Data.Configs.Tutorial;
using GeneralUtils;

namespace _Game.Scripts.Tutorial {
    public interface ITutorialController {
        public IEvent<TutorialConfig> TutorialStartEvent { get; }
        public IEvent<TutorialConfig> TutorialCompleteEvent { get; }
        public bool IsComplete(TutorialConfig tutorial);
    }
}