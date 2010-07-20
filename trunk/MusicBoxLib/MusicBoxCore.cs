using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using PandoraMusicBox.Engine.Encryption;

namespace PandoraMusicBox.Engine {
    public class MusicBoxCore {

        //private const int protocolVersion = 27;
        private const string baseUrl = "http://www.pandora.com/radio/xmlrpc/v27?";
        private const int port = 80;

        BlowfishCipher encrypter = new BlowfishCipher(PandoraCryptKeys.Out);
        BlowfishCipher decrypter = new BlowfishCipher(PandoraCryptKeys.In);

        public MusicBoxCore() {
            
        }

        public string RouteID {
            get { 
                if (_routeID == null)
                    _routeID = (DateTime.UtcNow.ToFileTime() % 10000000).ToString("0000000") + "P";
                
                return _routeID; 
            }
        } private string _routeID = null;

        public void AuthenticateListener() {
            string url = baseUrl + "rid=" + RouteID + "&authenticateListener";

            ASCIIEncoding encoder = new ASCIIEncoding();
            long time = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            string postStr = String.Format(XmlRequests.AuthenticateListener, time, "your@email.adr", "password");
            byte[] postData = encoder.GetBytes(encrypter.Encrypt(postStr));
            Console.WriteLine(postStr);

            WebRequest request = WebRequest.Create(url);
            request.ContentType = "text/xml";
            request.ContentLength = postData.Length;
            request.Method = "POST";

            Stream os = request.GetRequestStream();
            os.Write(postData, 0, postData.Length);
            os.Close();

            WebResponse response = request.GetResponse();
            if (request != null) {
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string reply = sr.ReadToEnd();
                Console.WriteLine(reply);
            }

        }


    }
}
