using System;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEngine;

namespace _Game.Scripts.UI.States.Impacts {
    public interface IAnimationImpact : IBaseImpact<Animation> { }

    [Serializable]
    public class AnimationWrapper : ComponentTypeWrapper<Animation, IAnimationImpact> { }

    [Serializable]
    public class AnimationImpactPlay : IAnimationImpact {
        public AnimationClip Clip;

        public void Apply(Animation target) {
            target.Play(Clip.name);
        }

        public void FillDefaultValues(Animation target) { }

        public override string ToString() {
            return Clip != null ? $"play {Clip.name}" : "";
        }
    }
}