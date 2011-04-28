using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PandoraMusicBox.Engine.Data {
    public class PandoraStation : PandoraData {
        public string Name {
            get;
            internal set;
        }

        public string Id {
            get;
            internal set;
        }

        public bool IsCreatedByUser {
            get;
            internal set;
        }

        public bool IsQuickMix {
            get;
            internal set;
        }
        
        internal PandoraStation(Dictionary<string, string> variables) {
            this.Variables = variables;
        }

        internal static List<PandoraStation> Parse(string xmlStr) {
            List<PandoraStation> stations = new List<PandoraStation>();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStr);

            // loop through each station in the XML document
            foreach (XmlNode currStationNode in xml.SelectNodes("//methodResponse/params/param/value/array/data/value/struct")) {
                Dictionary<string, string> varLookup = GetVariables(currStationNode);
                PandoraStation station = new PandoraStation(varLookup);
                station.Name = station["stationName"];
                station.Id = station["stationId"];
                station.IsCreatedByUser = station["isCreator"] == "1";
                station.IsQuickMix = station["isQuickMix"] == "1";

                stations.Add(station);
            }

            return stations;
        }
    }
}
