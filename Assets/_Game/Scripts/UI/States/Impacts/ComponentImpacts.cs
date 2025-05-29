using System;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEngine;

namespace _Game.Scripts.UI.States.Impacts {
    public interface IComponentImpact : IBaseImpact<Behaviour> { }

    [Serializable]
    public class ComponentWrapper : ComponentTypeWrapper<Behaviour, IComponentImpact> { }

    [Serializable]
    public class ComponentImpactSetEnabled : IComponentImpact {
        public bool Enabled;

        public void Apply(Behaviour target) {
            target.enabled = Enabled;
        }

        public void FillDefaultValues(Behaviour target) {
            Enabled = target.enabled;
        }

        public override string ToString() {
            return Enabled ? "enabled" : "disabled";
        }
    }
}