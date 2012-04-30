using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PandoraMusicBox.Engine.Data.Internal {
    internal class GetStationListResponse: PandoraData {

        [JsonProperty(PropertyName = "stations")]
        public List<PandoraStation> Stations {
            get;
            set;
        }


        [JsonProperty(PropertyName = "deviceModel")]
        public string Checksum {
            get;
            set;
        }
    }
}
