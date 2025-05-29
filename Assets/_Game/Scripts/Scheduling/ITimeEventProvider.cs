using System;
using GeneralUtils;

namespace _Game.Scripts.Scheduling {
    public interface ITimeEventProvider {
        public IEvent<float> FrameEvent { get; }
        public IDisposable SubscribeToPeriodicEvent(Action subscriber, float period);
        public IEvent<bool> PauseEvent { get; }
        public IEvent QuitEvent { get; }
    }
}