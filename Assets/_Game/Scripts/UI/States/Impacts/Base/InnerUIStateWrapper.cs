using System;

namespace _Game.Scripts.UI.States.Impacts.Base {
    [Serializable]
    public class InnerUIStateWrapper : ComponentTypeWrapper {
        public UIStateComponent InnerState;

        public override void Apply() {
            if (InnerState != null) {
                InnerState.Apply();
            }
        }

        public override string ToString() {
            if (InnerState == null) {
                return "not selected";
            }

            return "set state \"" + InnerState.DisplayName + "\"";
        }

        public override void FillDefaultValue(int impactIndex) { }

        public override void FillAllDefaultValue() { }
    }
}