using System;
using System.Collections.Generic;

namespace ByteBrewSDK
{
    [Serializable]
    public class ByteLog
    {
        public string category;
        public Dictionary<string, string> externalData;
    }
}
