using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using PandoraMusicBox.Engine.Encryption;
using System.Xml;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Web;
using PandoraMusicBox.Engine.Responses;
using PandoraMusicBox.Engine.Requests;
using PandoraMusicBox.Engine.Data;

namespace PandoraMusicBox.Engine {
    /// <summary>
    /// Low level class providing direct access to the Pandora API. 
    /// </summary>
    public class PandoraIO {
        private const string baseUrl = "tuner.pandora.com/services/json/?";

        internal static BlowfishCipher encrypter = new BlowfishCipher(PandoraCryptKeys.Out);
        internal static BlowfishCipher decrypter = new BlowfishCipher(PandoraCryptKeys.In);
        
        /// <summary>
        /// Initiates a Pandora session.
        /// </summary>
        /// <returns>A PandoraSession object that should be used with all other requests.</returns>
        public PandoraSession PartnerLogin() {
            return PartnerLogin(null);
        }

        /// <summary>
        /// Initiates a Pandora session.
        /// </summary>
        /// <param name="proxy">If not null, this proxy will be used to connect to the Pandora servers.</param>
        /// <returns>A PandoraSession object that should be used with all other requests.</returns>
        public PandoraSession PartnerLogin(WebProxy proxy) {
            return (PandoraSession) ExecuteRequest(new PartnerLoginRequest(), proxy);
        }


        /// <summary>
        /// Given the username and password, attempts to log into the Pandora music service.
        /// </summary>
        /// <returns>If login is successful, returns a PandoraUser object. If invalid username or password
        /// null is returned.</returns>
        public PandoraUser UserLogin(PandoraSession session, string username, string password) {
            return UserLogin(session, username, password, null);
        }

        /// <summary>
        /// Given the username and password, attempts to log into the Pandora music service.
        /// </summary>
        /// <returns>If login is successful, returns a PandoraUser object. If invalid username or password
        /// null is returned.</returns>
        public PandoraUser UserLogin(PandoraSession session, string username, string password, WebProxy proxy) {
            try {
                PandoraUser user = (PandoraUser) ExecuteRequest(new UserLoginRequest(session, username, password), proxy);
                session.User = user;
                return user;
            }
            catch (PandoraException e) {
                if (e.ErrorCode == ErrorCodeEnum.AUTH_INVALID_USERNAME_PASSWORD)
                    return null;

                throw;
            }
        }

        /// <summary>
        /// Retrieves a list of stations for the current user.
        /// </summary>
        public List<PandoraStation> GetStations(PandoraSession session) {
            return GetStations(session, null);
        }

        /// <summary>
        /// Retrieves a list of stations for the current user.
        /// </summary>
        public List<PandoraStation> GetStations(PandoraSession session, WebProxy proxy) {
            if (session == null || session.User == null)
                throw new PandoraException("User must be logged in to make this request.");

            GetStationListResponse response = (GetStationListResponse)ExecuteRequest(new GetStationListRequest(session), proxy);
            return response.Stations;
        }

        /// <summary>
        /// Retrieves all available genre stations.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public List<PandoraStationCategory> GetGenreStations(PandoraSession session) {
            return GetGenreStations(session, null);
        }

        public List<PandoraStationCategory> GetGenreStations(PandoraSession session, WebProxy proxy) {
            if (session == null || session.User == null)
                throw new PandoraException("User must be logged in to make this request.");

            GetGenreStationsResponse response = (GetGenreStationsResponse)ExecuteRequest(new GetGenreStationsRequest(session), proxy);
            return response.Categories;
        }

        /// <summary>
        /// Retrieves a playlist for the given station.
        /// </summary>
        public List<PandoraSong> GetSongs(PandoraSession session, PandoraStation station) {
            return GetSongs(session, station, null);
        }

        /// <summary>
        /// Retrieves a playlist for the given station.
        /// </summary>
        public List<PandoraSong> GetSongs(PandoraSession session, PandoraStation station, WebProxy proxy) {
            if (session == null || session.User == null) throw new PandoraException("User must be logged in to make this request.");

            GetPlaylistResponse response = (GetPlaylistResponse)ExecuteRequest(new GetPlaylistRequest(session, station.Token), proxy);
            return response.Songs;

        }

        public PandoraSongFeedback RateSong(PandoraSession session, PandoraStation station, PandoraSong song, PandoraRating rating) {
            return RateSong(session, station, song, rating, null);
        }

        public PandoraSongFeedback RateSong(PandoraSession session, PandoraStation station, PandoraSong song, PandoraRating rating, WebProxy proxy) {
            if (session == null || session.User == null) throw new PandoraException("User must be logged in to make this request.");

            PandoraSongFeedback feedbackObj = (PandoraSongFeedback)ExecuteRequest(new AddFeedbackRequest(session, station.Token, song.Token, rating == PandoraRating.Love), proxy);
            song.Rating = rating;

            return feedbackObj;
        }

        public void AddTiredSong(PandoraSession session, PandoraSong song) {
            AddTiredSong(session, song, null);
        }

        public void AddTiredSong(PandoraSession session, PandoraSong song, WebProxy proxy) {
            if (session == null || session.User == null) throw new PandoraException("User must be logged in to make this request.");

            ExecuteRequest(new SleepSongRequest(session, song.Token), proxy);
            song.TemporarilyBanned = true;
        }
          
        public void GetSongLength(PandoraUser user, PandoraSong song) {
            GetSongLength(user, song, null);
        }
        
        // estimate the length of each song based on file size
        public void GetSongLength(PandoraUser user, PandoraSong song, WebProxy proxy) {
            if (!IsValid(song, proxy)) {
                throw new PandoraException("Attempting to get song length for an expired song.");
            }
            
            WebRequest request = WebRequest.Create(song.AudioURL);
            if (proxy != null) request.Proxy = proxy;
            request.Method = "HEAD";

            using (WebResponse response = request.GetResponse()) {
                long bytes = response.ContentLength;
                int seconds = (int)((bytes * 8) / (user.AccountType == AccountType.PREMIUM ? 192000 : 128000));
                song.Length = new TimeSpan(0, 0, seconds);
            }
        }

        /// <summary>
        /// Returns true if the given PandoraSong is still valid. Links will expire after an unspecified
        /// number of hours.
        /// </summary>
        public bool IsValid(PandoraSong song) {
            return IsValid(song, null);
        }

        /// <summary>
        /// Returns true if the given PandoraSong is still valid. Links will expire after an unspecified
        /// number of hours.
        /// </summary>
        public bool IsValid(PandoraSong song, WebProxy proxy) {
            try {
                WebRequest request = WebRequest.Create(song.AudioURL);
                if (proxy != null) request.Proxy = proxy;
                request.Method = "HEAD";

                using (WebResponse response = request.GetResponse()) {
                    long bytes = response.ContentLength;
                }

                return true;
            }
            catch (WebException) {
                return false;
            }
        }

        private PandoraData ExecuteRequest(PandoraRequest request, WebProxy proxy) {
            try {
                ASCIIEncoding encoder = new ASCIIEncoding();

                // build url for request to pandora servers
                string prefix = request.IsSecure ? "https://" : "http://";
                string url = prefix + baseUrl;
                url += "method=" + request.MethodName;
                if (request.User != null) url += String.Format("&user_id={0}", HttpUtility.UrlEncode(request.User.Id));
                if (request.Session != null) {
                    url += String.Format("&auth_token={0}", HttpUtility.UrlEncode(request.UserAuthToken == null ? request.Session.PartnerAuthToken : request.UserAuthToken));
                    url += String.Format("&partner_id={0}", request.Session.PartnerId);
                }

                // build the post data for our request
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                string postStr = JsonConvert.SerializeObject(request, settings);
                byte[] postData = encoder.GetBytes(request.IsEncrypted ? encrypter.Encrypt(postStr) : postStr);

                // configure our connection
                ServicePointManager.Expect100Continue = false;
                WebRequest webRequest = WebRequest.Create(url);
                webRequest.ContentType = "text/xml";
                webRequest.ContentLength = postData.Length;
                webRequest.Method = "POST";
                if (proxy != null) webRequest.Proxy = proxy;

                // send request to remote servers
                Stream os = webRequest.GetRequestStream();
                os.Write(postData, 0, postData.Length);
                os.Close();

                // retrieve reply from servers
                using (WebResponse response = webRequest.GetResponse()) {
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    string replyStr = sr.ReadToEnd();
                    PandoraResponse reply = JsonConvert.DeserializeObject<PandoraResponse>(replyStr);
                    
                    // parse and throw any errors or return our result
                    if (!reply.Success) {
                        Console.WriteLine(url);
                        throw new PandoraException(String.Format("Received error code {0}: {1}", reply.ErrorCode, reply.ErrorMessage));
                    }

                    if (request.ReturnType == null) return null;
                    return (PandoraData)JsonConvert.DeserializeObject(reply.Result.ToString(), request.ReturnType);
                }
            }
            catch (Exception ex) {
                if (ex is PandoraException) throw;
                throw new PandoraException("Unexpected error communicating with server.", ex);
            }
        }
    }

    public enum PandoraRating {
        Love = 1,
        Unrated = 0,
        Hate = -1
    }
}
