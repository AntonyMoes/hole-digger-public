using System;
using _Game.Scripts.Data;
using _Game.Scripts.Data.DateTime;

namespace _Game.Scripts.Game.Resource {
    [Serializable]
    public class ResourceData : ListData<ResourceData>.IItem {
        public int configId;
        public int count;
        public SerializableDateTime nextAutoRefill = System.DateTime.UnixEpoch;

        public int ConfigId {
            get => configId;
            set => configId = value;
        }
    }
}