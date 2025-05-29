using System;
using _Game.Scripts.DI;

namespace _Game.Scripts.Time {
    public class LocalTimeProvider : ITimeProvider {
        public DateTime CurrentTime => DateTime.UtcNow;

        [Inject]
        public LocalTimeProvider() { }
    }
}