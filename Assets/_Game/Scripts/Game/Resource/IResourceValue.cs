using System.Collections.Generic;
using System.Linq;

namespace _Game.Scripts.Game.Resource {
    public interface IResourceValue {
        public IReadOnlyList<Resource> Value { get; }
    }

    public static class ResourceValues {
        public static IResourceValue Combine(IResourceValue value1, IResourceValue value2) {
            var queue = new Queue<Resource>(value1.Value.Concat(value2.Value));
            var result = new List<Resource>();
            while (queue.Count > 0) {
                var resource = queue.Dequeue();
                var index = result.FindIndex(r => r.Config == resource.Config);
                if (index == -1) {
                    result.Add(resource);
                } else {
                    result[index] = result[index].Combine(resource);
                }
            }

            return new ResourceValue(result.ToArray());
        }

        public static IResourceValue Multiply(this IResourceValue value, int multiplier) {
            return new ResourceValue(value.Value.Select(resource => resource * multiplier).ToArray());
        }

        private class ResourceValue : IResourceValue {
            public IReadOnlyList<Resource> Value { get; }

            public ResourceValue(IReadOnlyList<Resource> value) {
                Value = value;
            }
        }
    }
}