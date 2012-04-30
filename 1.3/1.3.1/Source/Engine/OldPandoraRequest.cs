using System;
using System.Collections.Generic;
using System.Text;

namespace PandoraMusicBox.Engine {
    class OldPandoraRequest {
        public string URLSuffix {
            get { return _urlSuffix; }
        } private string _urlSuffix;

        public string JsonRequest {
            get { return _jsonRequest; }
        } private string _jsonRequest;

        public OldPandoraRequest(string urlSuffix, string jsonRequest) {
            this._urlSuffix = urlSuffix;
            this._jsonRequest = jsonRequest;
        }

        #region Predefines Server Requests

        public static readonly OldPandoraRequest PartnerLogin = new OldPandoraRequest(
            "method=auth.partnerLogin",

            "{\"deviceModel\":\"android-generic\",\"username\":\"android\",\"includeUrls\":true,\"password\":\"AC7IBG09A3DTSYM4R41UJWL07VLN8JI7\",\"version\":\"5\"}"
        );

        public static readonly OldPandoraRequest UserLogin = new OldPandoraRequest(
            "method=auth.userLogin",

            "{{\"loginType\":\"user\",\"username\":\"{0}\",\"password\":\"{1}\",\"partnerAuthToken\":\"{2}\",\"syncTime\":{3}}}"
        );

        public static readonly OldPandoraRequest GetStations = new OldPandoraRequest(
            "&method=getStations",

            "<?xml version=\"1.0\"?>" +
            "<methodCall><methodName>station.getStations</methodName>" +
            "<params><param><value><int>{0}</int></value></param>" +
            "<param><value><string>{1}</string></value></param>" +
            "</params></methodCall>"
        );

        public static readonly OldPandoraRequest GetFragment = new OldPandoraRequest(
            "&method=getFragment&arg1={0}&arg2=0&arg3=&arg4=&arg5={1}&arg6=0&arg7=0",

            "<?xml version=\"1.0\"?>" +
            "<methodCall><methodName>playlist.getFragment</methodName>" +
            "<params><param><value><int>{0}</int></value></param>" +
            "<param><value><string>{1}</string></value></param>" +
            "<param><value><string>{2}</string></value></param>" +
            "<param><value><string>0</string></value></param>" +
            "<param><value><string></string></value></param>" +
            "<param><value><string></string></value></param>" +
            "<param><value><string>{3}</string></value></param>" +
            "<param><value><string>0</string></value></param>" +
            "<param><value><string>0</string></value></param>" +
            "</params></methodCall>"
        );

        public static readonly OldPandoraRequest RateSong = new OldPandoraRequest(
            "&method=addFeedback&arg1={0}&arg2={1}&arg3={2}",
            
            "<?xml version=\"1.0\"?>" +
            "<methodCall><methodName>station.addFeedback</methodName>" +
            "<params><param><value><int>{0}</int></value></param>" +
            "<param><value><string>{1}</string></value></param>" +
            "<param><value><string>{2}</string></value></param>" +
            "<param><value><string>{3}</string></value></param>" +
            "<param><value><boolean>{4}</boolean></value></param>" +
            "</params></methodCall>"
            );

        public static readonly OldPandoraRequest AddTiredSong = new OldPandoraRequest(
            "&method=addTiredSong&arg1={0}",
            
            "<?xml version=\"1.0\"?>" +
            "<methodCall><methodName>listener.addTiredSong</methodName><params>" +
            "<param><value><int>{0}</int></value></param>" +
            "<param><value><string>{1}</string></value></param>" +
            "<param><value><string>{2}</string></value></param>" +
            "</params></methodCall>"
            );

        public static readonly OldPandoraRequest CanListen = new OldPandoraRequest(
            "&method=canListen&arg1={0}", // auth code

            "<?xml version=\"1.0\"?><methodCall><methodName>listener.canListen</methodName><params>" +
            "<param><value><int>{0}</int></value></param>" + // time
            "<param><value><string></string></value></param>" +
            "<param><value><string>{1}</string></value></param>" + // auth code
            "</params></methodCall>"
        );

        #endregion
    }
}
