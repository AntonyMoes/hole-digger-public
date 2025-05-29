using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Condition;
using _Game.Scripts.Data.Configs.Meta.Transaction;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Price;
using _Game.Scripts.Game.Resource;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Condition {
    [Serializable, SerializeReferenceMenuItem(MenuName = "CanPerformAnyTransaction")]
    public class CanPerformAnyTransactionConditionValue : ConditionValue {
        [SerializeField] private TransactionConfig[] _transactions;

        protected override ICondition PerformGetCondition(IContainer container) {
            return container.Create<CanPerformAnyTransactionCondition>(
                (IReadOnlyList<TransactionConfig>) _transactions);
        }

        private class CanPerformAnyTransactionCondition : Condition {
            private readonly ITransactionController _transactionController;
            private readonly IReadOnlyList<TransactionConfig> _transactions;
            private readonly IContainer _container;

            [Inject]
            public CanPerformAnyTransactionCondition(IResourceController resourceController,
                ITransactionController transactionController, IReadOnlyList<TransactionConfig> transactions,
                IContainer container) {
                _transactionController = transactionController;
                _transactions = transactions;
                _container = container;
                foreach (var resourceHolder in resourceController.Resources.Select(resourceController.GetResource)) {
                    resourceHolder.Amount.Subscribe(_ => OnResourceUpdate());
                }
                
                OnResourceUpdate();
            }

            private void OnResourceUpdate() {
                MutableValue.Value = _transactions
                    .Select(config => config.TryGetTransaction(_container))
                    .Where(transaction => transaction != null)
                    .Any(_transactionController.CanPerform);
            }
        }
    }
}