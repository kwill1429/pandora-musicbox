using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PandoraMusicBox.Engine.Data;

namespace PandoraMusicBox.Engine.Requests {
    internal class PartnerLoginRequest: PandoraRequest {
        public override string MethodName {
            get { return "auth.partnerLogin"; }
        }

        public override Type ReturnType {
            get { return typeof(PandoraSession); }
        }

        public override bool IsSecure {
            get { return true; }
        }

        public override bool IsEncrypted {
            get { return false; }
        }

        [JsonProperty(PropertyName = "deviceModel")]
        public string DeviceModel {
            get { return "android-generic"; }
        }

        [JsonProperty(PropertyName = "username")]
        public string UserName {
            get { return "android"; }
        }

        [JsonProperty(PropertyName = "includeUrls")]
        public bool IncludeUrls {
            get { return true; }
        }
        
        [JsonProperty(PropertyName = "password")]
        public string Password {
            get { return "AC7IBG09A3DTSYM4R41UJWL07VLN8JI7"; }
        }

        [JsonProperty(PropertyName = "version")]
        public string Version {
            get { return "5"; }
        }
    }
}
