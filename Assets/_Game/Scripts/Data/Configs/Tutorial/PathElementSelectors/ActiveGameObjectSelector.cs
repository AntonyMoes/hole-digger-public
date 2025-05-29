using System;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Tutorial.PathElementSelectors {
    [Serializable, SerializeReferenceMenuItem(MenuName = "ActiveGameObject")]
    public class ActiveGameObjectSelector : PathElementSelector {
        [SerializeField] private bool _active;

        public override bool Passes(GameObject element, IContainer container) {
            return element.activeSelf == _active;
        }
    }
}