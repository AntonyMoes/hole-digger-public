using System;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEngine.UI;

namespace _Game.Scripts.UI.States.Impacts {
    public interface IButtonImpact : IBaseImpact<Button> { }

    [Serializable]
    public class ButtonWrapper : ComponentTypeWrapper<Button, IButtonImpact> { }

    [Serializable]
    public class ButtonImpactSetOnClick : IButtonImpact {
        public Button.ButtonClickedEvent Interactable;

        public void Apply(Button target) {
            target.onClick = Interactable;
        }

        public void FillDefaultValues(Button target) {
            Interactable = target.onClick;
        }

        public override string ToString() {
            return "override on click";
        }
    }
}