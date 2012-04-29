using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PandoraMusicBox.Engine.Encryption;
using Newtonsoft.Json;

namespace PandoraMusicBox.Engine.Data {
    public enum AccountType { BASIC, TRIAL, PREMIUM, EXPIRED_SUBSCRIBER }

    [JsonObject(MemberSerialization.OptIn)]
    public class PandoraUser: PandoraData {
        BlowfishCipher cipher = new BlowfishCipher(PandoraCryptKeys.PW);
        
        [JsonProperty(PropertyName = "username")]
        public string Name {
            get;
            internal set;
        }

        internal string EncryptedPassword {
            get;
            set;
        }

        internal string Password {
            get {
                try { return cipher.Decrypt(EncryptedPassword); }
                catch (Exception) { }
                return "";
            }
            set {
                if (value != null) EncryptedPassword = cipher.Encrypt(value);
            }
        }

        [JsonProperty(PropertyName = "canListen")]
        public bool CanListen {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "userAuthToken")]
        public string AuthorizationToken {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "userId")]
        public string Id {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "hasAudioAds")]
        public bool RequiresAds {
            get;
            internal set;
        }

        public int AdInterval {
            get;
            internal set;
        }

        public AccountType AccountType {
            get;
            internal set;
        }

        [JsonProperty(PropertyName = "age")]
        public int Age {
            get;
            set;
        }

        [JsonProperty(PropertyName = "zip")]
        public int ZipCode {
            get;
            internal set;
        }

        public TimeSpan TimeoutInterval {
            get {
                if (this.AccountType == AccountType.BASIC)
                    return new TimeSpan(1, 0, 0);
                else
                    return new TimeSpan(5, 0, 0);
            }
        }
    }
}
