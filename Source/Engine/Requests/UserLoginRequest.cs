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

        [JsonProperty(PropertyName = "includeDemographics")]
        public bool IncludeDemographics {
            get { return true; }
        }

        [JsonProperty(PropertyName = "includePandoraOneInfo")]
        public bool IncludePandoraOneInfo {
            get { return true; }
        }

        [JsonProperty(PropertyName = "includeAdAttributes")]
        public bool IncludeAdAttributes {
            get { return true; }
        }

        [JsonProperty(PropertyName = "returnStationList")]
        public bool ReturnStationList {
            get { return false; }
        }

        [JsonProperty(PropertyName = "returnGenreStations")]
        public bool ReturnGenreStations {
            get { return false; }
        }

        [JsonProperty(PropertyName = "includeStationArtUrl")]
        public bool IncludeStationArtUrl {
            get { return true; }
        }

        [JsonProperty(PropertyName = "complimentarySponsorSupported")]
        public bool ComplimentarySponsorSupported {
            get { return true; }
        }

        [JsonProperty(PropertyName = "includeSubscriptionExpiration")]
        public bool IncludeSubscriptionExpiration {
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
