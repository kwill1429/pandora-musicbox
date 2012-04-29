using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PandoraMusicBox.Engine.Data.Internal {
    [JsonObject(MemberSerialization.OptIn)]
    internal class PandoraResponse: PandoraData {
        public bool Success {
            get { return Status == "ok"; }
        }

        [JsonProperty(PropertyName = "stat")]
        public string Status {
            get;
            set;
        }

        [JsonProperty(PropertyName = "result")]
        public JToken Result {
            get;
            set;
        }

        [JsonProperty(PropertyName = "message")]
        public string ErrorMessage {
            get;
            set;
        }

        [JsonProperty(PropertyName = "code")]
        public int ErrorCode {
            get;
            set;
        }
    }
}
