using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PandoraMusicBox.Engine.Data {
    public enum AccountType { BASIC, TRIAL, PREMIUM }

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

        public AccountType AccountType {
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
            
            if (user["listenerState"] == "REGISTERED") user.AccountType = AccountType.BASIC;
            if (user["listenerState"] == "COMPLIMENTARY") user.AccountType = AccountType.TRIAL;
            if (user["listenerState"] == "SUBSCRIBER") user.AccountType = AccountType.PREMIUM;

            return user;
        }

    }
}
