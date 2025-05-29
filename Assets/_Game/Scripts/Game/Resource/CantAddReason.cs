using System.Collections.Generic;
using _Game.Scripts.Data.Configs.Meta;

namespace _Game.Scripts.Game.Resource {
    public class CantAddReason {
        public readonly IReadOnlyList<ResourceConfig> Resources;
        public readonly CantAddReasonType ReasonType;

        public CantAddReason(IReadOnlyList<ResourceConfig> resources, CantAddReasonType reasonType) {
            Resources = resources;
            ReasonType = reasonType;
        }
    }
}