using _Game.Scripts.DI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Scripts.Input {
    public static class InputFactory {
        public static IInputController CreateController(EventSystem eventSystem, LayerMask uiMask, IContainer container) {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            // TODO
#endif

            return container.Create<MouseInputController>(eventSystem, uiMask);
        }
    }
}