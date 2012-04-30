using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PandoraMusicBox.Engine.Data;
using PandoraMusicBox.Engine.Responses;

namespace PandoraMusicBox.Engine.Requests {
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
