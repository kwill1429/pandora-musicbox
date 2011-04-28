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

        public int AdInterval {
            get;
            internal set;
        }

        public AccountType AccountType {
            get;
            internal set;
        }

        public int BirthYear {
            get;
            internal set;
        }

        public int Age {
            get { return DateTime.Now.Year - BirthYear; }
        }

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

        internal static PandoraUser Parse(string xmlStr) {
            int tmp;

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStr);

            Dictionary<string, string> varLookup = GetVariables(xml.SelectSingleNode("//struct"));

            PandoraUser user = new PandoraUser(varLookup);
            user.Name = user["username"];
            user.AuthorizationToken = user["authToken"];
            user.WebAuthorizationToken = user["webAuthToken"];
            user.ListenerId = user["listenerId"];
            
            if (int.TryParse(user["autoplayAdInterval"], out tmp))
                user.AdInterval = tmp;
            else 
                user.AdInterval = 15;
            
            if (int.TryParse(user["zipcode"], out tmp))
                user.ZipCode = tmp;
            else
                user.ZipCode = 0;

            if (int.TryParse(user["birthYear"], out tmp))
                user.BirthYear = tmp;
            else
                user.BirthYear = 0;
            
            if (user["listenerState"] == "REGISTERED") user.AccountType = AccountType.BASIC;
            if (user["listenerState"] == "COMPLIMENTARY") user.AccountType = AccountType.TRIAL;
            if (user["listenerState"] == "SUBSCRIBER") user.AccountType = AccountType.PREMIUM;
            if (user["listenerState"] == "EXPIRED_SUBSCRIBER") user.AccountType = AccountType.EXPIRED_SUBSCRIBER;

            return user;
        }

    }
}
