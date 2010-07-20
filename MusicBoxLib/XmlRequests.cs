using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PandoraMusicBox.Engine {
    class XmlRequests {
        public const string AuthenticateListener =
            "<?xml version=\"1.0\"?><methodCall>" +
            "<methodName>listener.authenticateListener</methodName>" +
            "<params><param><value><int>{0}</int></value></param>" +
            "<param><value><string>{1}</string></value></param>" +
            "<param><value><string>{2}</string></value></param>" +
            "</params></methodCall>";
    }
}
