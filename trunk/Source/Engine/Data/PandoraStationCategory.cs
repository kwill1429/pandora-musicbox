using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PandoraMusicBox.Engine.Data {
    public class PandoraStationCategory: PandoraData {
        [JsonProperty(PropertyName = "categoryName")]
        public String Name {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "stations")]
        public List<PandoraStation> Stations {
            get;
            internal set;
        }
    }
}
