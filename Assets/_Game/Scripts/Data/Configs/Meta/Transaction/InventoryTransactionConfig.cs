using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta.Transaction {
    [CreateAssetMenu(menuName = Configs.MetaMenuItem + nameof(InventoryTransactionConfig),
        fileName = nameof(InventoryTransactionConfig))]
    public class InventoryTransactionConfig : SimpleTransactionConfig {
        [SerializeField] private ResourceConfig _inventoryResource;
        public ResourceConfig InventoryResource => _inventoryResource;
    }
}