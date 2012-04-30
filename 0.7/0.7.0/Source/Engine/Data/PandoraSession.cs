using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PandoraMusicBox.Engine.Data {

    [JsonObject(MemberSerialization.OptIn)]
    public class PandoraSession: PandoraData {
        public PandoraUser User {
            get;
            internal set;
        }
        
        public long TimeOffset {
            get {
                if (_timeOffset == null) {
                    string decryptedTime = PandoraIO.decrypter.Decrypt(EncryptedSyncTime).Substring(4, 10);
                    long serverTime;
                    if (long.TryParse(decryptedTime, out serverTime)) {
                        _timeOffset = GetTime() - serverTime;
                    }
                }

                if (_timeOffset == null) return 0;
                else return (long)_timeOffset;
            }
        } private long? _timeOffset = null;

        public long ReferenceTime {
            get {
                if (_referenceTime == 0) {
                    try { _referenceTime = int.Parse(PandoraIO.decrypter.Decrypt(EncryptedSyncTime).Substring(4, 10)); }
                    catch (Exception) { }
                }

                return _referenceTime;
            }
        } private long _referenceTime = 0;

        [JsonProperty(PropertyName = "partnerId")]
        public int PartnerId { get; internal set; }

        [JsonProperty(PropertyName = "partnerAuthToken")]
        public string PartnerAuthToken { get; internal set; }

        [JsonProperty(PropertyName = "syncTime")]
        public string EncryptedSyncTime { get; internal set; }

        public long GetSyncTime() {
            return GetTime() - TimeOffset;
        }

        public long GetTime() {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }


    }
}
