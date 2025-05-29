using System;
using _Game.Scripts.Tutorial;
using GeneralUtils;

namespace _Game.Scripts.Data.Configs.Tutorial.StepFinishConditions {
    [Serializable]
    public abstract class TutorialStepFinishCondition : ITutorialStepFinishCondition {
        protected readonly Event Finish = new Event();
        public IEvent FinishEvent => Finish;

        public abstract void Init(ITutorialStepFinishCondition.Parameters parameters);
    }
}