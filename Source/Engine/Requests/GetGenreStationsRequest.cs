using System;
using System.Collections.Generic;
using System.Text;
using PandoraMusicBox.Engine.Data;
using Newtonsoft.Json;
using PandoraMusicBox.Engine.Responses;

namespace PandoraMusicBox.Engine.Requests {
    internal class GetGenreStationsRequest: PandoraRequest {
        public override string MethodName {
            get { return "station.getGenreStations"; }
        }

        public override Type ReturnType {
            get { return typeof(GetGenreStationsResponse); }
        }

        public override bool IsSecure {
            get { return false; }
        }

        public override bool IsEncrypted {
            get { return true; }
        }

        [JsonProperty(PropertyName = "includeChecksum")]
        public bool IncludeCheckSum {
            get { return true; }
        }

        [JsonProperty(PropertyName = "includeStationArtUrl")]
        public bool IncludeStationArtUrl {
            get { return true; }
        }

        public GetGenreStationsRequest(PandoraSession session) :
            base(session) {
        }
    }
}
