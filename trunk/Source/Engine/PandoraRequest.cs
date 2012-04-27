﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PandoraMusicBox.Engine {
    class PandoraRequest {
        public string URLSuffix {
            get { return _urlSuffix; }
        } private string _urlSuffix;

        public string XmlRpcRequest {
            get { return _xmlRpcRequest; }
        } private string _xmlRpcRequest;

        public PandoraRequest(string urlSuffix, string xmlRpcRequest) {
            this._urlSuffix = urlSuffix;
            this._xmlRpcRequest = xmlRpcRequest;
        }

        #region Predefines Server Requests

        public static readonly PandoraRequest Sync = new PandoraRequest(
            "&method=sync",

            "<?xml version=\"1.0\"?><methodCall>" +
	        "<methodName>misc.sync</methodName>" +
            "<params><param><value><string>eb1ba959649711ab20c405dde29c5c58</string></value></param></params>" +
            "</methodCall>"
        );

        public static readonly PandoraRequest AuthenticateListener = new PandoraRequest(
            "&authenticateListener",

            "<?xml version=\"1.0\"?><methodCall>" +
            "<methodName>listener.authenticateListener</methodName>" +
            "<params><param><value><int>{0}</int></value></param>" +
            "<param><value><string>00000000000000000000000000000000</string></value></param>" +
            "<param><value><string>{1}</string></value></param>" +
            "<param><value><string>{2}</string></value></param>" +
            "<param><value><string>html5tuner</string></value></param>" + 
            "<param><value><string></string></value></param>" + 
            "<param><value><string></string></value></param>" + 
            "<param><value><string>HTML5</string></value></param>" +
            "<param><value><boolean>1</boolean></value></param>" +
            "</params></methodCall>"
        );

        public static readonly PandoraRequest GetStations = new PandoraRequest(
            "&method=getStations",

            "<?xml version=\"1.0\"?>" +
            "<methodCall><methodName>station.getStations</methodName>" +
            "<params><param><value><int>{0}</int></value></param>" +
            "<param><value><string>{1}</string></value></param>" +
            "</params></methodCall>"
        );

        public static readonly PandoraRequest GetFragment = new PandoraRequest(
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

        public static readonly PandoraRequest RateSong = new PandoraRequest(
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

        public static readonly PandoraRequest AddTiredSong = new PandoraRequest(
            "&method=addTiredSong&arg1={0}",
            
            "<?xml version=\"1.0\"?>" +
            "<methodCall><methodName>listener.addTiredSong</methodName><params>" +
            "<param><value><int>{0}</int></value></param>" +
            "<param><value><string>{1}</string></value></param>" +
            "<param><value><string>{2}</string></value></param>" +
            "</params></methodCall>"
            );

        public static readonly PandoraRequest CanListen = new PandoraRequest(
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
