using System;
using System.Linq;
using UnityEngine;

namespace _Game.Scripts.UI.States.Impacts.Base {
    [Serializable]
    public abstract class ComponentTypeWrapper<TComponent, TImpact> : ComponentTypeWrapper
        where TComponent : UnityEngine.Object
        where TImpact : IBaseImpact<TComponent> {
        public TComponent[] Targets;

        [SerializeReference]
        public TImpact[] Impacts;

        public sealed override void Apply() {
            for (var i = 0; i < Impacts.Length; i++) {
                for (var t = 0; t < Targets.Length; t++) {
                    Impacts[i].Apply(Targets[t]);
                }
            }
        }

        public sealed override void FillDefaultValue(int impactIndex) {
            if (impactIndex < 0 || impactIndex >= Impacts.Length) {
                return;
            }

            var impact = Impacts[impactIndex];
            if (impact == null) {
                return;
            }

            var target = Targets.FirstOrDefault();
            if (target == null) {
                return;
            }

            impact.FillDefaultValues(target);
        }

        public sealed override void FillAllDefaultValue() {
            var target = Targets.FirstOrDefault();
            if (target == null) {
                return;
            }

            for (var i = 0; i < Impacts.Length; i++) {
                Impacts[i].FillDefaultValues(target);
            }
        }

        public override string ToString() {
            if (Targets == null || Impacts == null || Targets.Length == 0 || Targets[0] == null) {
                return "no targets";
            }

            var target = Targets.Length switch {
                1 => "\"" + Targets[0]?.name + "\"",
                _ => "(" + Targets.Length + ")"
            };

            var impact = Impacts.Length switch {
                1 => Impacts[0]?.ToString(),
                _ => Impacts.Length + " impacts"
            };

            return target + " set " + impact;
        }
    }

    [Serializable]
    public abstract class ComponentTypeWrapper {
        public abstract void Apply();
        public abstract void FillDefaultValue(int impactIndex);
        public abstract void FillAllDefaultValue();
    }
}