using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PandoraMusicBox.Engine.Data.Internal {
    internal class UserLoginRequest: PandoraRequest {

        public override string MethodName {
            get { return "auth.userLogin"; }
        }

        public override Type ReturnType {
            get { return typeof(PandoraUser); }
        }

        public override bool IsSecure {
            get { return true; }
        }

        public override bool IsEncrypted {
            get { return true; }
        }

        [JsonProperty(PropertyName = "partnerAuthToken")]
        public string PartnerAuthToken {
            get {
                if (Session != null) return Session.PartnerAuthToken;
                return null;
            }
        }

        [JsonProperty(PropertyName = "loginType")]
        public string LoginType {
            get { return "user"; }
        }

        [JsonProperty(PropertyName = "username")]
        public string UserName {
            get;
            set;
        }

        [JsonProperty(PropertyName = "password")]
        public string Password {
            get;
            set;
        }

        public UserLoginRequest(PandoraSession session, string username, string password):
            base(session) {

            this.UserName = username;
            this.Password = password;
        }
    }
}
