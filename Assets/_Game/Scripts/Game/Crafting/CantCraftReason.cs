using _Game.Scripts.Game.Resource;
using JetBrains.Annotations;

namespace _Game.Scripts.Game.Crafting {
    public class CantCraftReason {
        public readonly CantCraftReasonType ReasonType;
        [CanBeNull] public readonly CantAddReason CantAddReason;

        public CantCraftReason(CantCraftReasonType reasonType, [CanBeNull] CantAddReason cantAddReason = null) {
            ReasonType = reasonType;
            CantAddReason = cantAddReason;
        }
    }
}