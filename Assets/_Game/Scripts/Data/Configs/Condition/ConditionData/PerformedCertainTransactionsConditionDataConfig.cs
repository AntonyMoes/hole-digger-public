using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Condition;
using _Game.Scripts.Data.Configs.Meta.Transaction;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Price;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Condition.ConditionData {
    [CreateAssetMenu(menuName = Configs.ConditionMenuItem + nameof(PerformedCertainTransactionsConditionDataConfig),
        fileName = nameof(PerformedCertainTransactionsConditionDataConfig))]
    public class PerformedCertainTransactionsConditionDataConfig : ConditionDataConfig<bool> {
        [SerializeField] private TransactionConfig[] _trackedTransactions;

        protected override IConditionData<bool> PerformGetValue(IContainer container) {
            return container.Create<PerformedCertainTransactionsConditionData>(container, _trackedTransactions);
        }

        private class PerformedCertainTransactionsConditionData : ConditionData<bool> {
            private readonly ITransactionController _transactionController;
            private readonly IReadOnlyList<TransactionConfig> _trackedTransactions;

            [Inject]
            public PerformedCertainTransactionsConditionData(ITransactionController transactionController,
                IReadOnlyList<TransactionConfig> trackedTransactions) {
                _transactionController = transactionController;
                _trackedTransactions = trackedTransactions;
                transactionController.TransactionPerformEvent.Subscribe(OnTransactionPerformed);
            }

            private void OnTransactionPerformed(Transaction transaction) {
                if (_trackedTransactions.Contains(transaction.Config)) {
                    MutableValue.Value = true;
                    _transactionController.TransactionPerformEvent.Unsubscribe(OnTransactionPerformed);
                }
            }
        }
    }
}