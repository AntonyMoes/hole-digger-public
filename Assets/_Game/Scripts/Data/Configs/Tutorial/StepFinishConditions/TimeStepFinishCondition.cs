using System;
using System.Collections;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.Scheduling;
using _Game.Scripts.Tutorial;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Tutorial.StepFinishConditions {
    [Serializable, SerializeReferenceMenuItem(MenuName = "Time")]
    public class TimeStepFinishCondition : TutorialStepFinishCondition {
        [SerializeField] private float _time;

        public override void Init(ITutorialStepFinishCondition.Parameters parameters) {
            parameters.Container.Get<IScheduler>().StartCoroutine(Wait());
        }

        private IEnumerator Wait() {
            yield return new WaitForSeconds(_time);
            Finish.Invoke();
        }
    }
}