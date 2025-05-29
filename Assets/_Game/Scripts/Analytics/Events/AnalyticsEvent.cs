using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace _Game.Scripts.Analytics.Events {
    public abstract class AnalyticsEvent {
        public readonly string Name;

        protected AnalyticsEvent(string name) {
            Name = name;
        }

        public Dictionary<string, object> GetData() {
            return GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(info => info.GetCustomAttributes(typeof(LogFieldAttribute)).Count() != 0)
                .ToDictionary(info => info.Name, info => info.GetValue(this));
        }
    }
}