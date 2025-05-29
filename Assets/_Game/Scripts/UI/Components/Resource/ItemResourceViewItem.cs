using UnityEngine;

namespace _Game.Scripts.UI.Components.Resource {
    public class ItemResourceViewItem : MonoBehaviour {
        [SerializeField] private GameObject _filling;

        public void SetFilled(bool filled) {
            _filling.SetActive(filled);
        }
    }
}