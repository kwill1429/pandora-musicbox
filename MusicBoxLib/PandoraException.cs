using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PandoraMusicBox.Engine {
    public enum ErrorCodeEnum {
        UNKNOWN,
        APPLICATION_ERROR,

        AUTH_INVALID_USERNAME_PASSWORD
    }

    public class PandoraException: Exception {
        /// <summary>
        /// If identifiable, the raw error code from the pandora.com servers.
        /// </summary>
        public ErrorCodeEnum ErrorCode {
            get { return _errorCode; }
        } protected ErrorCodeEnum _errorCode;

        /// <summary>
        /// Human readable details about the error this object represents.
        /// </summary>
        public override string Message {
            get { return _message; }
        } protected string _message;

        /// <summary>
        /// Create an exception from an error code and message provided by the Pandora servers.
        /// If the error code is recognized, the ErrorCode field will be populated.
        /// </summary>
        /// <param name="errorCodeStr"></param>
        /// <param name="message"></param>
        public PandoraException(string errorCodeStr, string message) {
            try {
                _message = message;
                _errorCode = (ErrorCodeEnum) Enum.Parse(typeof(ErrorCodeEnum), errorCodeStr);
            } catch (Exception) {
                _errorCode = ErrorCodeEnum.UNKNOWN;
                _message = errorCodeStr + ": " + message;
            }
        }

        /// <summary>
        /// Creates an exception instance based on a supplied inner exception and a 
        /// message describing the situation. Should only be used for unexpected errors.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public PandoraException(string message, Exception innerException) :
            base(message, innerException) {

            _message = message;
            _errorCode = ErrorCodeEnum.APPLICATION_ERROR;
        }

        /// <summary>
        /// Creates an exception instance based on a supplied message describing the 
        /// situation. Should only be used for unexpected errors.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public PandoraException(Exception innerException) :
            this("Unexpected library error. So Sorry!", innerException) {

            _message = "Unexpected library error. So Sorry!";
        }

        /// <summary>
        /// Based on the XML input assumed to be retrieved from the pandora.com XML-RPC interface,
        /// this method will return a PandoraException object if an error occured, otherwise it will 
        /// return null.
        /// </summary>
        /// <param name="xmInput"></param>
        /// <returns></returns>
        public static PandoraException ParseError(string xmInput) {
            XmlDocument xml = new XmlDocument();

            try {
                xml.LoadXml(xmInput);
                foreach (XmlNode currNode in xml.SelectNodes("/methodResponse/fault/value/struct/member")) {
                    if (currNode["name"].InnerText == "faultString") {
                        string errorCode = currNode["value"].InnerText.Split('|')[2];
                        string errorMsg = currNode["value"].InnerText.Split('|')[3];
                        return new PandoraException(errorCode, errorMsg);
                    }
                }

                return null;
            }
            catch (Exception e) {
                return new PandoraException("Failed to parse response XML.", e);
            }
        }
    }
}
