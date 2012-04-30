using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PandoraMusicBox.Engine.Data;

namespace PandoraMusicBox.Engine.Requests {
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class PandoraRequest {
        public abstract string MethodName {
            get;
        }

        public abstract Type ReturnType {
            get;
        }

        public abstract bool IsSecure {
            get;
        }

        public abstract bool IsEncrypted {
            get;
        }

        public PandoraUser User {
            get;
            set;
        }

        public PandoraSession Session {
            get;
            set;
        }

        [JsonProperty(PropertyName = "userAuthToken")]
        public string UserAuthToken {
            get {
                if (User != null) return User.AuthorizationToken;
                return null;
            }
        }

        [JsonProperty(PropertyName = "syncTime")]
        public long? SyncTime {
            get {
                if (Session != null) return Session.GetSyncTime();
                return null;
            }
        }

        public PandoraRequest() :
            this(null) {
        }

        public PandoraRequest(PandoraSession session) {
            this.Session = session;
            this.User = session == null ? null : session.User;
        }
    }
}
