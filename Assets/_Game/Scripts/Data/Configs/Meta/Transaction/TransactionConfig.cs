using _Game.Scripts.DI;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Meta.Transaction {
    public abstract class TransactionConfig : Config {
        [SerializeField] private string _logReason;
        public string LogReason => _logReason;

        [CanBeNull] public abstract Game.Price.Transaction TryGetTransaction(IContainer container);
    }
}