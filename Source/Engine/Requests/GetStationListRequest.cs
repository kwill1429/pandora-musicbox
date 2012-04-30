using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PandoraMusicBox.Engine.Data.Internal {
    internal class GetStationListRequest: PandoraRequest {
        public override string MethodName {
            get { return "user.getStationList"; }
        }

        public override Type ReturnType {
            get { return typeof(GetStationListResponse); }
        }

        public override bool IsSecure {
            get { return false; }
        }

        public override bool IsEncrypted {
            get { return true; }
        }

        public GetStationListRequest(PandoraSession session) :
            base(session) {
        }
    }
}
