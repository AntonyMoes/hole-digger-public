using UnityEngine;

namespace _Game.Scripts.Game.Level.Props {
    public class NoTorchZone : MonoBehaviour {
        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent<Torch>(out var torch)) {
                torch.Toggle(false, true);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.TryGetComponent<Torch>(out var torch)) {
                torch.Toggle(true);
            }
        }
    }
}