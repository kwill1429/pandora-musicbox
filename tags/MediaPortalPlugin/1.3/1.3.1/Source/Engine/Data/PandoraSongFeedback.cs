using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PandoraMusicBox.Engine.Data {
    public class PandoraSongFeedback: PandoraData {
        [JsonProperty(PropertyName = "isPositive")]
        public bool IsPositive {
            get;
            set;
        }

        [JsonProperty(PropertyName = "artistName")]
        public string Artist {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "songName")]
        public string Title {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "feedbackId")]
        public string FeedbackId {
            get;
            internal set;
        }
    }
}
