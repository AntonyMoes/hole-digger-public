using _Game.Scripts.Data.Configs.Meta.Transaction;

namespace _Game.Scripts.Game.Price {
    public class Transaction {
        public readonly TransactionConfig Config;
        public readonly IPrice Price;
        public readonly IReward Reward;

        public Transaction(IPrice price, IReward reward, TransactionConfig config) {
            Config = config;
            Price = price;
            Reward = reward;
        }
    }
}