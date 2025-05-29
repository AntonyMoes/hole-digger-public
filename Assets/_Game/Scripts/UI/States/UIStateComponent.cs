using _Game.Scripts.UI.States.Impacts.Base;
using UnityEngine;

namespace _Game.Scripts.UI.States {
    public class UIStateComponent : MonoBehaviour {
        // [EditorOnly]
        [HideInInspector]
        public string DisplayName;

        [SerializeReference]
        private ComponentTypeWrapper[] _wrappers;

        public void Apply() {
            foreach (var wrapper in _wrappers) {
                wrapper.Apply();
            }

#if UNITY_EDITOR
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
#endif
        }
    }
}