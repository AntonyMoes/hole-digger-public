using System;

namespace _Game.Scripts.Time {
    public interface ITimeProvider {
        public DateTime CurrentTime { get; }
    }
}