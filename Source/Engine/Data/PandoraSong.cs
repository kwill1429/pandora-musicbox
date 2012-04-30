using System;
using System.Collections.Generic;
using System.Text;
using PandoraMusicBox.Engine.Encryption;
using System.Xml;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace PandoraMusicBox.Engine.Data {
    public class PandoraSong: PandoraData {
        public class AudioUrlInfo {
            [JsonProperty(PropertyName = "bitrate")]
            public string Bitrate {
                get;
                internal set;
            }

            [JsonProperty(PropertyName = "encoding")]
            public string Encoding {
                get;
                internal set;
            }

            [JsonProperty(PropertyName = "audioUrl")]
            public string Url {
                get;
                internal set;
            }

            [JsonProperty(PropertyName = "protocol")]
            public string Protocol {
                get;
                set;
            }
        }

        private static BlowfishCipher decrypter = new BlowfishCipher(PandoraCryptKeys.In);

        public bool IsAdvertisement {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "trackToken")]
        public string Token {
            get;
            internal set;
        }


        [JsonProperty(PropertyName = "artistName")]
        public string Artist {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "albumName")]
        public string Album {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "songName")]
        public string Title {
            get;
            internal set;
        }

        public string AudioURL {
            get {
                if (AudioUrlMap == null || AudioUrlMap.Count == 0)
                    return null;

                if (AudioUrlMap.ContainsKey("highQuality"))
                    return AudioUrlMap["highQuality"].Url.Trim();
                if (AudioUrlMap.ContainsKey("mediumQuality"))
                    return AudioUrlMap["mediumQuality"].Url.Trim();
                if (AudioUrlMap.ContainsKey("lowQuality"))
                    return AudioUrlMap["lowQuality"].Url.Trim();

                return null;
            }
        }

        [JsonProperty(PropertyName = "audioUrlMap")]
        public Dictionary<string, AudioUrlInfo> AudioUrlMap {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "albumArtUrl")]
        public string AlbumArtLargeURL {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "albumDetailUrl")]
        public string AlbumDetailsURL {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "songRating")]
        private int NumericRating {
            get {
                switch (Rating) {
                    case PandoraRating.Love:
                        return 1;
                    case PandoraRating.Unrated:
                        return 0;
                    case PandoraRating.Hate:
                        return -1;
                    default:
                        return 0;
                }
            }
            set {
                if (value == 1) Rating = PandoraRating.Love;
                else Rating = PandoraRating.Unrated;
            }
        }

        public PandoraRating Rating {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "trackGain")]
        private string TrackGainStr {
            get;
            set;
        }

        public float TrackGain {
            get {
                float rtn = 0;
                float.TryParse(TrackGainStr, out rtn);
                return rtn;
            }
        }

        public bool TemporarilyBanned {
            get;
            internal set;
        }

        public TimeSpan Length {
            get;
            internal set;
        }
    }
}
