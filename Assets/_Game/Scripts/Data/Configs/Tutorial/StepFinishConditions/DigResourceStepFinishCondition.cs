using System;
using System.Collections.Generic;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.Game.Level;
using _Game.Scripts.Tutorial;

namespace _Game.Scripts.Data.Configs.Tutorial.StepFinishConditions {
    [Serializable, SerializeReferenceMenuItem(MenuName = "DigResource")]
    public class DigResourceStepFinishCondition : TutorialStepFinishCondition {
        public override void Init(ITutorialStepFinishCondition.Parameters parameters) {
            var levelController = parameters.Container.Get<ILevelController>();
            levelController.PrioritizeResources.Value = true;
            levelController.LevelCells.Subscribe(OnCellsUpdate);

            void OnCellsUpdate(IReadOnlyList<ILevelController.LevelCell> _) {
                levelController.LevelCells.Unsubscribe(OnCellsUpdate);
                levelController.PrioritizeResources.Value = false;
                Finish.Invoke();
            }
        }
    }
}