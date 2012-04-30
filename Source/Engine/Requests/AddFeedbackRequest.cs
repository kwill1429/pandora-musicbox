using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PandoraMusicBox.Engine.Data.Internal {
    internal class AddFeedbackRequest: PandoraRequest {
        public override string MethodName {
            get { return "station.addFeedback"; }
        }

        public override Type ReturnType {
            get { return typeof(PandoraSongFeedback); }
        }

        public override bool IsSecure {
            get { return false; }
        }

        public override bool IsEncrypted {
            get { return true; }
        }

        [JsonProperty(PropertyName = "stationToken")]
        public string StationToken {
            get;
            set;
        }

        [JsonProperty(PropertyName = "trackToken")]
        public string TrackToken {
            get;
            set;
        }

        [JsonProperty(PropertyName = "isPositive")]
        public bool IsPositive {
            get;
            set;
        }

        public AddFeedbackRequest(PandoraSession session, string stationToken, string trackToken, bool positive) :
            base(session) {
            this.StationToken = stationToken;
            this.TrackToken = trackToken;
        }
    }
}
