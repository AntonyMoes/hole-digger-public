using System;
using _Game.Scripts.Data.Configs.Meta;

namespace _Game.Scripts.Game.Resource {
    public class Resource {
        public ResourceConfig Config { get; }
        public int Amount { get; }

        public Resource(ResourceConfig config, int amount) {
            Config = config;
            Amount = amount;
        }

        public Resource Combine(Resource other) {
            if (other.Config != Config) {
                throw new ArgumentException($"Wrong resources to combine: {Config.ConfigId} && {other.Config.ConfigId}",
                    nameof(other));
            }

            return new Resource(Config, Amount + other.Amount);
        }

        public static Resource operator *(Resource resource, int value) {
            return new Resource(resource.Config, resource.Amount * value);
        }

        public static Resource operator *(int value, Resource resource) {
            return resource * value;
        }
    }
}