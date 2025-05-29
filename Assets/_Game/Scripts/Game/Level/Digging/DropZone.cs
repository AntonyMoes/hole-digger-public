using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging {
    public class DropZone : MonoBehaviour {
        private void OnTriggerExit(Collider other) {
            if (other.TryGetComponent<Drop>(out var drop)) {
                drop.Remove();
            }
        }
    }
}