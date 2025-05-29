using System;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEngine;

namespace _Game.Scripts.UI.States.Impacts {
    public interface ICanvasGroupImpact : IBaseImpact<CanvasGroup> { }

    [Serializable]
    public class CanvasGroupWrapper : ComponentTypeWrapper<CanvasGroup, ICanvasGroupImpact> { }

    [Serializable]
    public class CanvasGroupImpactSetAlpha : ICanvasGroupImpact {
        [Range(0f, 1f)]
        public float Alpha;

        public void Apply(CanvasGroup target) {
            target.alpha = Alpha;
        }

        public void FillDefaultValues(CanvasGroup target) {
            Alpha = target.alpha;
        }

        public override string ToString() {
            return "alpha = " + Alpha;
        }
    }

    [Serializable]
    public class CanvasGroupImpactSetInteractable : ICanvasGroupImpact {
        public bool Interactable;

        public void Apply(CanvasGroup target) {
            target.interactable = Interactable;
        }

        public void FillDefaultValues(CanvasGroup target) {
            Interactable = target.interactable;
        }

        public override string ToString() {
            return "interactable = " + Interactable;
        }
    }
}