using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PandoraMusicBox.Engine.Data.Internal {
    internal class GetPlaylistRequest: PandoraRequest {
        public override string MethodName {
            get { return "station.getPlaylist"; }
        }

        public override Type ReturnType {
            get { return typeof(GetPlaylistResponse); }
        }

        public override bool IsSecure {
            get { return true; }
        }

        public override bool IsEncrypted {
            get { return true; }
        }

        [JsonProperty(PropertyName = "stationToken")]
        public string StationToken {
            get;
            set;
        }

        public GetPlaylistRequest(PandoraSession session, string stationToken) :
            base(session) {
            this.StationToken = stationToken;
        }
    }
}
