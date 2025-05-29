using System;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEngine.UI;

namespace _Game.Scripts.UI.States.Impacts {
    public interface ISelectableImpact : IBaseImpact<Selectable> { }

    [Serializable]
    public class SelectableWrapper : ComponentTypeWrapper<Selectable, ISelectableImpact> { }

    [Serializable]
    public class SelectableImpactSetInteractible : ISelectableImpact {
        public bool Interactable;

        public void Apply(Selectable target) {
            target.interactable = Interactable;
        }

        public void FillDefaultValues(Selectable target) {
            Interactable = target.interactable;
        }

        public override string ToString() {
            return "interactable = " + Interactable;
        }
    }
}