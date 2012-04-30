using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PandoraMusicBox.Engine.Data;

namespace PandoraMusicBox.Engine.Responses {
    internal class GetPlaylistResponse: PandoraData {
        [JsonProperty(PropertyName = "items")]
        public List<PandoraSong> Songs {
            get;
            set;
        }
    }
}
