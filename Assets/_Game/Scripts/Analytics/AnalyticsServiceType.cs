using System;

namespace _Game.Scripts.Analytics {
    [Flags]
    public enum AnalyticsServiceType {
        None = 0,
        Firebase = 1 << 0,
        ByteBrew = 1 << 1,
    }
}