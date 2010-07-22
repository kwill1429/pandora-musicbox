using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PandoraMusicBox.Engine.Data {
    public class PandoraData {

        public Dictionary<string, string> Variables {
            get;
            protected set;
        }
        
        public string this[string key] {
            get {
                if (!Variables.ContainsKey(key)) 
                    throw new PandoraException("XML-RPC response missing expected value: '" + key + "'");

                return Variables[key];
            }
        }

        protected static Dictionary<string, string> GetVariables(XmlNode xml) {
            Dictionary<string, string> lookup = new Dictionary<string, string>();
            
            try {
                foreach (XmlNode currNode in xml.SelectNodes("member")) 
                    lookup.Add(currNode["name"].InnerText, currNode["value"].InnerText);                
            }
            catch (Exception e) {
                throw new PandoraException("Failed to parse response XML.", e);
            }

            return lookup;
        }

    }
}
