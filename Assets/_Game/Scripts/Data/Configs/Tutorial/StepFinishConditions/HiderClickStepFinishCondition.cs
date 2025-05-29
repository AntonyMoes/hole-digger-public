using System;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.Input;
using _Game.Scripts.Tutorial;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Tutorial.StepFinishConditions {
    [Serializable, SerializeReferenceMenuItem(MenuName = "HiderClick")]
    public class HiderClickStepFinishCondition : TutorialStepFinishCondition {
        [SerializeField] private bool _clickHiderHole;

        public override void Init(ITutorialStepFinishCondition.Parameters parameters) {
            if (parameters.Hider == null) {
                parameters.InitErrorEvent.Invoke("Can't use hider click condition because the hider is null!");
                return;
            }

            var inputController = parameters.Container.Get<IInputController>();
            inputController.UITapEvent.Subscribe(OnUITap);
            inputController.NonUITapEvent.Subscribe(OnTap);

            void OnUITap(Vector2 position, GameObject uiObject) {
                if (_clickHiderHole && uiObject != null && uiObject.transform.IsChildOf(parameters.Hider.transform)) {
                    return;
                }

                inputController.UITapEvent.Unsubscribe(OnUITap);
                inputController.NonUITapEvent.Unsubscribe(OnTap);
                Finish.Invoke();
            }

            void OnTap(Vector2 position) {
                inputController.UITapEvent.Unsubscribe(OnUITap);
                inputController.NonUITapEvent.Unsubscribe(OnTap);
                Finish.Invoke();
            }
        }
    }
}