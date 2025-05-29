using UnityEngine;

namespace _Game.Scripts.Data.Configs {
    [CreateAssetMenu(menuName = Configs.MenuItem + nameof(UIConfig), fileName = nameof(UIConfig))]
    public class UIConfig : Config {
        [SerializeField] private LayerMask _uiMask;
        public LayerMask UIMask => _uiMask;

        [SerializeField] private Sprite _timeIcon;
        public Sprite TimeIcon => _timeIcon;
    }
}