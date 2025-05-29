using System;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.Game.Level;
using _Game.Scripts.Tutorial;
using GeneralUtils;

namespace _Game.Scripts.Data.Configs.Tutorial.StepFinishConditions {
    [Serializable, SerializeReferenceMenuItem(MenuName = "CollectDrop")]
    public class CollectDropStepFinishCondition : TutorialStepFinishCondition {
        public override void Init(ITutorialStepFinishCondition.Parameters parameters) {
            var levelController = parameters.Container.Get<ILevelController>();
            levelController.CollectDropEvent.SubscribeOnce(_ => Finish.Invoke());
        }
    }
}