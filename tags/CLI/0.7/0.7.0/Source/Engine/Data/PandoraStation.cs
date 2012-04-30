using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace PandoraMusicBox.Engine.Data {
    public class PandoraStation : PandoraData {
        [JsonProperty(PropertyName = "stationName")]
        public string Name {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "stationId")]
        public string Id {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "stationToken")]
        public string Token {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "isQuickMix")]
        public bool IsQuickMix {
            get;
            internal set;
        }
        
    }
}
