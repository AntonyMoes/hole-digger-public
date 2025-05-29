using System;
using UnityEngine;

namespace _Game.Scripts.UI.States {
    [Serializable]
    public class UIStateSwitcher {
        [SerializeField]
        private UIState _activeState;

        [SerializeField]
        private UIState _inactiveState;

        public void SetActive(bool active) {
            var state = active ? _activeState : _inactiveState;
            state.Apply();
        }
    }
}