using System;

namespace _Game.Scripts.Utils {
    public class Singleton<TSelf> : IDisposable where TSelf : Singleton<TSelf> {
        public static TSelf Instance { get; private set; }

        protected Singleton() {
            if (Instance != null) {
                throw new Exception($"{typeof(Singleton<TSelf>)} already has an instance!");
            }

            Instance = (TSelf) this;
        }

        public void Dispose() {
            PerformDispose();
            Instance = default;
        }

        protected virtual void PerformDispose() { }
    }
}