using System;
using _Game.Scripts.Condition;
using _Game.Scripts.Data.Configs.Tutorial;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using _Game.Scripts.Tutorial;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Condition {
    [Serializable, SerializeReferenceMenuItem(MenuName = "TutorialComplete")]
    public class TutorialCompleteConditionValue : ConditionValue {
        [SerializeField] private TutorialConfig _tutorial;

        protected override ICondition PerformGetCondition(IContainer container) {
            return container.Create<TutorialCompleteCondition>(_tutorial);
        }

        private class TutorialCompleteCondition : Condition {
            private readonly ITutorialController _tutorialController;
            private readonly TutorialConfig _tutorial;

            [Inject]
            public TutorialCompleteCondition(ITutorialController tutorialController, TutorialConfig tutorial) {
                _tutorialController = tutorialController;
                _tutorial = tutorial;

                if (tutorialController.IsComplete(_tutorial)) {
                    MutableValue.Value = true;
                } else {
                    tutorialController.TutorialCompleteEvent.Subscribe(OnTutorialComplete);
                }
            }

            private void OnTutorialComplete(TutorialConfig tutorial) {
                if (tutorial == _tutorial) {
                    _tutorialController.TutorialCompleteEvent.Unsubscribe(OnTutorialComplete);
                    MutableValue.Value = true;
                }
            }
        }
    }
}