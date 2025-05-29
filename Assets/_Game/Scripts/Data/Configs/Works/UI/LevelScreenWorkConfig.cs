using _Game.Scripts.UI.Level;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Works.UI {
    [CreateAssetMenu(menuName = Configs.UIWorkMenuItem + nameof(LevelScreenWorkConfig),
        fileName = nameof(LevelScreenWorkConfig))]
    public class LevelScreenWorkConfig : UIWorkConfig<LevelScreenPresenter, ILevelScreenView, LevelScreenParameters> {
        [SerializeField] private WorkConfig _levelWork;
        [SerializeField] private WorkConfig _inventoryWork;
        [SerializeField] private WorkConfig _shopWork;

        protected override LevelScreenParameters Parameters => new LevelScreenParameters {
            UIPrefab = UIPrefab,
            LevelWork = _levelWork,
            InventoryWork = _inventoryWork,
            ShopWork = _shopWork
        };
    }

    public struct LevelScreenParameters : IUIParameters {
        public GameObject UIPrefab { get; set; }
        public WorkConfig LevelWork { get; set; }
        public WorkConfig InventoryWork { get; set; }
        public WorkConfig ShopWork { get; set; }
    }
}