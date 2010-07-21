using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using PandoraMusicBox.Engine.Encryption;
using System.Xml;

namespace PandoraMusicBox.Engine {
    public class MusicBoxCore {
        private const string baseUrl = "http://www.pandora.com/radio/xmlrpc/v27?";

        BlowfishCipher encrypter = new BlowfishCipher(PandoraCryptKeys.Out);
        BlowfishCipher decrypter = new BlowfishCipher(PandoraCryptKeys.In);

        private string RouteID {
            get { 
                if (_routeID == null)
                    _routeID = (DateTime.UtcNow.ToFileTime() % 10000000).ToString("0000000") + "P";
                
                return _routeID; 
            }
        } private string _routeID = null;

        public bool AuthenticateListener(string username, string password) {
            try {
                long time = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                XmlDocument response = ExecuteRequest(PandoraRequest.AuthenticateListener, time, username, password);
            }
            catch (PandoraException e) {
                if (e.ErrorCode == ErrorCodeEnum.AUTH_INVALID_USERNAME_PASSWORD)
                    return false;

                throw;
            }

            return true;
        }

        private XmlDocument ExecuteRequest(PandoraRequest request, params object[] paramList) {
            ASCIIEncoding encoder = new ASCIIEncoding();

            // build app specific info for request to pandora servers
            string url = baseUrl + "rid=" + RouteID + request.URLSuffix;
            string postStr = String.Format(request.XmlRpcRequest, paramList);
            byte[] postData = encoder.GetBytes(encrypter.Encrypt(postStr));

            // configure request object
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.ContentType = "text/xml";
            webRequest.ContentLength = postData.Length;
            webRequest.Method = "POST";

            // send request to remote servers
            Stream os = webRequest.GetRequestStream();
            os.Write(postData, 0, postData.Length);
            os.Close();

            // parse response
            WebResponse response = webRequest.GetResponse();
            if (request != null) {
                // retrieve reply from servers
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string reply = sr.ReadToEnd();

                // check for error response
                PandoraException ex = PandoraException.ParseError(reply);
                if (ex != null) throw ex;

                // build return object
                

            }

            return null;
        }




    }
}
