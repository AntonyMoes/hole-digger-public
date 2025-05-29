using System;
using System.Collections;
using UnityEngine;

namespace _Game.Scripts.Scheduling {
    public interface ICoroutineRunner {
        public void RunInMainThread(Action action);
        public void StartCoroutine(IEnumerator enumerator, Action<Coroutine> onStarted = null);
        public void StopCoroutine(Coroutine coroutine);
    }
}