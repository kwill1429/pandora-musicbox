using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PandoraMusicBox.Engine.Data {
    public class PandoraUser: PandoraData {

        internal PandoraUser(Dictionary<string, string> variables) {
            this.Variables = variables;
        }

        public string Name {
            get;
            internal set;
        }

        public string AuthorizationToken {
            get;
            internal set;
        }

        public string WebAuthorizationToken {
            get;
            internal set;
        }

        public string ListenerId {
            get;
            internal set;
        }

        internal static PandoraUser Parse(string xmlStr) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStr);

            Dictionary<string, string> varLookup = GetVariables(xml.SelectSingleNode("//struct"));

            PandoraUser user = new PandoraUser(varLookup);
            user.Name = user["username"];
            user.AuthorizationToken = user["authToken"];
            user.WebAuthorizationToken = user["webAuthToken"];
            user.ListenerId = user["listenerId"];

            return user;
        }

    }
}
