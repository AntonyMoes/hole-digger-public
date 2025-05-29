using System;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEngine;

namespace _Game.Scripts.UI.States.Impacts {
    public interface IAnimatorImpact : IBaseImpact<Animator> { }

    [Serializable]
    public class AnimatorWrapper : ComponentTypeWrapper<Animator, IAnimatorImpact> { }

    [Serializable]
    public class AnimatorImpactSetBool : IAnimatorImpact {
        public string BoolName;
        public bool BoolValue;

        public void Apply(Animator target) {
            target.SetBool(BoolName, BoolValue);
        }

        public void FillDefaultValues(Animator target) {
            BoolValue = target.GetBool(BoolName);
        }

        public override string ToString() {
            return BoolName + " = " + BoolValue;
        }
    }

    [Serializable]
    public class AnimatorImpactSetInteger : IAnimatorImpact {
        public string IntegerName;
        public int IntegerValue;

        public void Apply(Animator target) {
            target.SetInteger(IntegerName, IntegerValue);
        }

        public void FillDefaultValues(Animator target) {
            IntegerValue = target.GetInteger(IntegerName);
        }

        public override string ToString() {
            return IntegerName + " = " + IntegerValue;
        }
    }

    [Serializable]
    public class AnimatorImpactSetFloat : IAnimatorImpact {
        public string FloatName;
        public float FloatValue;

        public void Apply(Animator target) {
            target.SetFloat(FloatName, FloatValue);
        }

        public void FillDefaultValues(Animator target) {
            FloatValue = target.GetFloat(FloatName);
        }

        public override string ToString() {
            return FloatName + " = " + FloatValue;
        }
    }

    [Serializable]
    public class AnimatorImpactSetTrigger : IAnimatorImpact {
        public string TriggerName;

        public void Apply(Animator target) {
            target.SetTrigger(TriggerName);
        }

        public void FillDefaultValues(Animator target) { }

        public override string ToString() {
            return "trigger \"" + TriggerName + "\"";
        }
    }

    [Serializable]
    public class AnimatorImpactResetTrigger : IAnimatorImpact {
        public string TriggerName;

        public void Apply(Animator target) {
            target.ResetTrigger(TriggerName);
        }

        public void FillDefaultValues(Animator target) { }

        public override string ToString() {
            return "reset trigger \"" + TriggerName + "\"";
        }
    }
}