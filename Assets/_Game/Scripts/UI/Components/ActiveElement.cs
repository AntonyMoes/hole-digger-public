using _Game.Scripts.UI.States;
using UnityEngine;

namespace _Game.Scripts.UI.Components {
    public class ActiveElement : MonoBehaviour {
        [SerializeField] private UIState _active;
        [SerializeField] private UIState _inactive;

        public bool State { get; private set; }

        public void SetActive(bool active) {
            State = active;
            var state = active ? _active : _inactive;
            state.Apply();
        }
    }
}