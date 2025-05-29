using System;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using _Game.Scripts.UI.Components;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Tutorial.PathElementSelectors {
    [Serializable, SerializeReferenceMenuItem(MenuName = "ActiveElement")]
    public class ActiveElementSelector : PathElementSelector {
        [SerializeField] private bool _active;

        public override bool Passes(GameObject element, IContainer container) {
            return element.TryGetComponent<ActiveElement>(out var activeElement) && activeElement.State == _active;
        }
    }
}