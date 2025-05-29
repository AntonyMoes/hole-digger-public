using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Input {
    public interface IInputController {
        public IEvent<Vector2, GameObject> UITapEvent { get; }
        public IEvent<Vector2> NonUITapEvent { get; }
    }
}