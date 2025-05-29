using _Game.Scripts.Data.Configs.Meta.Reward;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Leveling;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta.Transaction {
    [CreateAssetMenu(menuName = Configs.MetaMenuItem + nameof(LeveledEntityTransactionConfig),
        fileName = nameof(LeveledEntityTransactionConfig))]
    public class LeveledEntityTransactionConfig : TransactionConfig {
        [SerializeField] private LeveledEntityConfig _leveledEntityConfig;
        [SerializeReferenceMenu]
        [SerializeReference] private Price.Price[] _prices;

        public override Game.Price.Transaction TryGetTransaction(IContainer container) {
            var levelingController = container.Get<ILevelingController>();
            var level = levelingController.GetLevelData(_leveledEntityConfig).Level.Value;
            if (!levelingController.CanAddLevel(_leveledEntityConfig) || level >= _prices.Length) {
                return null;
            }

            var price = _prices[level];
            var reward = new LevelReward(_leveledEntityConfig);
            return new Game.Price.Transaction(price, reward, this);
        }
    }
}