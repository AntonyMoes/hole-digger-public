using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta.Transaction {
    [CreateAssetMenu(menuName = Configs.MetaMenuItem + nameof(SimpleTransactionConfig),
        fileName = nameof(SimpleTransactionConfig))]
    public class SimpleTransactionConfig : TransactionConfig {
        [SerializeReferenceMenu]
        [SerializeReference] private Price.Price _price;
        [SerializeReferenceMenu]
        [SerializeReference] private Reward.Reward _reward;

        public override Game.Price.Transaction TryGetTransaction(IContainer container) {
            return new Game.Price.Transaction(_price, _reward, this);
        }
    }
}