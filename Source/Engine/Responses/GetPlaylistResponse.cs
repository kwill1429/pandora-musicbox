using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PandoraMusicBox.Engine.Data.Internal {
    internal class GetPlaylistResponse: PandoraData {
        [JsonProperty(PropertyName = "items")]
        public List<PandoraSong> Songs {
            get;
            set;
        }
    }
}
