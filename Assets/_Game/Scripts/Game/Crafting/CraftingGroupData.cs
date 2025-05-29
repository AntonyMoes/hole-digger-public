using System;
using System.Collections.Generic;
using _Game.Scripts.Data;
using _Game.Scripts.Data.DateTime;

namespace _Game.Scripts.Game.Crafting {
    [Serializable]
    public class CraftingGroupData : ListData<CraftingGroupData>.IItem {
        public int configId;
        public List<CraftingProcess> processes = new List<CraftingProcess>();

        [Serializable]
        public class CraftingProcess {
            public int configId;
            public int count;
            public SerializableDateTime completesAt = System.DateTime.UnixEpoch;
        }

        public int ConfigId {
            get => configId;
            set => configId = value;
        }
    }
}