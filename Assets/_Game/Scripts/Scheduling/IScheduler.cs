using System;

namespace _Game.Scripts.Scheduling {
    public interface IScheduler : ICoroutineRunner, ITimeEventProvider, IDisposable { }
}