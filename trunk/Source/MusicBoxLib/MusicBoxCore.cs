using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using PandoraMusicBox.Engine.Encryption;
using System.Xml;
using PandoraMusicBox.Engine.Data;

namespace PandoraMusicBox.Engine {
    /// <summary>
    /// Low level class providing direct access to the Pandora API. 
    /// </summary>
    public class MusicBoxCore {
        private const string baseUrl = "http://www.pandora.com/radio/xmlrpc/v27?";

        BlowfishCipher encrypter = new BlowfishCipher(PandoraCryptKeys.Out);

        private string RouteId {
            get { 
                if (_routeId == null)
                    _routeId = (DateTime.UtcNow.ToFileTime() % 10000000).ToString("0000000") + "P";
                
                return _routeId;
            }
        } private string _routeId = null;

        /// <summary>
        /// Given the username and password, attempts to log into the Pandora music service.
        /// </summary>
        /// <returns>If login is successful, returns a PandoraUser object. If invalid username or password
        /// null is returned.</returns>
        public PandoraUser AuthenticateListener(string username, string password) {
            try {        
                string reply = ExecuteRequest(null, PandoraRequest.AuthenticateListener, username, password);
                return PandoraUser.Parse(reply);
            }
            catch (PandoraException e) {
                if (e.ErrorCode == ErrorCodeEnum.AUTH_INVALID_USERNAME_PASSWORD)
                    return null;

                throw;
            }
        }

        /// <summary>
        /// Retrieves a list of stations for the given user.
        /// </summary>
        public List<PandoraStation> GetStations(PandoraUser user) {
            if (user == null)
                throw new PandoraException("User must be logged in to make this request.");

            string reply = ExecuteRequest(user, PandoraRequest.GetStations);
            return PandoraStation.Parse(reply);
        }

        /// <summary>
        /// Retrieves a playlist for the given station.
        /// </summary>
        public List<PandoraSong> GetSongs(PandoraUser user, PandoraStation station) {
            if (user == null)
                throw new PandoraException("User must be logged in to make this request.");

            string reply = ExecuteRequest(user, PandoraRequest.GetFragment, station.Id, "mp3-hifi");
            return PandoraSong.Parse(reply);

        }

        private string ExecuteRequest(PandoraUser user, PandoraRequest request, params object[] paramList) {
            string reply;

            try {
                ASCIIEncoding encoder = new ASCIIEncoding();

                // build method specific info for request to pandora servers
                string url = baseUrl + "rid=" + RouteId;
                if (user != null) url += "&lid=" + user.ListenerId;
                url += String.Format(request.URLSuffix, paramList);

                // build parameter list for the xml-rpc request
                int index = 0;
                object[] xmlParams = new object[paramList.Length + 2];
                xmlParams[index++] = GetTime();
                if (user != null) xmlParams[index++] = user.AuthorizationToken;
                foreach(object currParam in paramList)
                    xmlParams[index++] = currParam;

                string postStr = String.Format(request.XmlRpcRequest, xmlParams);
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

                // retrieve reply from servers
                WebResponse response = webRequest.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                reply = sr.ReadToEnd();
            }
            catch (Exception ex) {
                throw new PandoraException("Unexpected error communicating with server.", ex);
            }

            // check for error response
            PandoraException error = PandoraException.ParseError(reply);
            if (error != null) throw error;

            return reply;
        }

        private long GetTime() {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }


    }
}
