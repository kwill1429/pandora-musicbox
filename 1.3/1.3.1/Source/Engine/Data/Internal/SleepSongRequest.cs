using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PandoraMusicBox.Engine.Data.Internal {
    internal class SleepSongRequest: PandoraRequest {
        public override string MethodName {
            get { return "user.sleepSong"; }
        }

        public override Type ReturnType {
            get { return null; }
        }

        public override bool IsSecure {
            get { return false; }
        }

        public override bool IsEncrypted {
            get { return true; }
        }

        [JsonProperty(PropertyName = "trackToken")]
        public string TrackToken {
            get;
            set;
        }

        public SleepSongRequest(PandoraSession session, string trackToken) :
            base(session) {
            this.TrackToken = trackToken;
        }
    }
}
