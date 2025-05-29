using System;
using _Game.Scripts.DI;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Tutorial.PathElementSelectors {
    [Serializable]
    public abstract class PathElementSelector {
        public abstract bool Passes(GameObject element, IContainer container);
    }
}