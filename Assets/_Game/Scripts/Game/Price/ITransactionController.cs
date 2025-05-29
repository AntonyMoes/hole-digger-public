using GeneralUtils;

namespace _Game.Scripts.Game.Price {
    public interface ITransactionController {
        public IEvent<Transaction> TransactionPerformEvent { get; }

        public bool CanPerform(Transaction transaction);
        public bool TryPerform(Transaction transaction);
    }
}