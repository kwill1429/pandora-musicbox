using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PandoraMusicBox.Engine.Data;

namespace PandoraMusicBox.Engine.Responses {
    internal class GetGenreStationsResponse: PandoraData {
        [JsonProperty(PropertyName = "categories")]
        public List<PandoraStationCategory> Categories {
            get;
            set;
        }

        [JsonProperty(PropertyName = "checksum")]
        public string Checksum {
            get;
            set;
        }
    }
}
