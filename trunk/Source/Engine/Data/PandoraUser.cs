using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PandoraMusicBox.Engine.Encryption;

namespace PandoraMusicBox.Engine.Data {
    public enum AccountType { BASIC, TRIAL, PREMIUM, EXPIRED_SUBSCRIBER }

    public class PandoraUser: PandoraData {
        BlowfishCipher cipher = new BlowfishCipher(PandoraCryptKeys.In);

        internal PandoraUser(Dictionary<string, string> variables) {
            this.Variables = variables;
        }

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

        //public int RemainingHours

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
            if (user["listenerState"] == "EXPIRED_SUBSCRIBER") user.AccountType = AccountType.EXPIRED_SUBSCRIBER;

            return user;
        }

    }
}
