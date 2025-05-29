using System;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEngine;

namespace _Game.Scripts.UI.States.Impacts {
    public interface IGameObjectImpact : IBaseImpact<GameObject> { }

    [Serializable]
    public class GameObjectWrapper : ComponentTypeWrapper<GameObject, IGameObjectImpact> { }

    [Serializable]
    public class GameObjectImpactSetActive : IGameObjectImpact {
        public bool Active;

        public void Apply(GameObject target) {
            target.SetActive(Active);
        }

        public void FillDefaultValues(GameObject target) {
            Active = target.activeSelf;
        }

        public override string ToString() {
            return Active ? "active" : "inactive";
        }
    }

    [Serializable]
    public class GameObjectImpactForceRebuildLayout : IGameObjectImpact {
        public void Apply(GameObject target) {
            target.ForceRebuildLayout();
        }

        public void FillDefaultValues(GameObject target) { }

        public override string ToString() {
            return "rebuild all layouts";
        }
    }
}