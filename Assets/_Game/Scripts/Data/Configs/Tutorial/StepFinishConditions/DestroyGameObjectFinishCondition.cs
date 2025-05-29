using System;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.Scheduling;
using _Game.Scripts.Tutorial;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Tutorial.StepFinishConditions {
    [Serializable, SerializeReferenceMenuItem(MenuName = "DestroyGameObject")]
    public class DestroyGameObjectFinishCondition : TutorialStepFinishCondition {
        [SerializeField] private ReferenceElementPath _referenceElementPath;

        public override void Init(ITutorialStepFinishCondition.Parameters parameters) {
            var gameObject = _referenceElementPath.Find(null, parameters.Container);
            if (gameObject == null) {
                parameters.InitErrorEvent.Invoke(
                    $"{nameof(DestroyGameObjectFinishCondition)} could not find element with path \"{_referenceElementPath}\"");
                return;
            }

            var scheduler = parameters.Container.Get<IScheduler>();
            scheduler.FrameEvent.Subscribe(OnFrame);

            void OnFrame(float deltaTime) {
                if (gameObject == null) {
                    scheduler.FrameEvent.Unsubscribe(OnFrame);
                    Finish.Invoke();
                }
            }
        }
    }
}