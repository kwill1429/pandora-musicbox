using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PandoraMusicBox.Engine.Responses;

namespace PandoraMusicBox.Engine {
    public enum ErrorCodeEnum {
        UNKNOWN = -1,
        APPLICATION_ERROR = -2,

        INTERNAL = 0,
        MAINTENANCE_MODE = 1,
        URL_PARAM_MISSING_METHOD = 2,
        URL_PARAM_MISSING_AUTH_TOKEN = 3,
        URL_PARAM_MISSING_PARTNER_ID = 4,
        URL_PARAM_MISSING_USER_ID = 5, 
        SECURE_PROTOCOL_REQUIRED = 6,
        CERTIFICATE_REQUIRED = 7,
        PARAMETER_TYPE_MISMATCH = 8,
        PARAMETER_MISSING = 9,
        PARAMETER_VALUE_INVALID = 10,
        INCOMPATIBLE_VERSION = 11,
        LICENSE_RESTRICTION = 12,
        INSUFFICIENT_CONNECTIVITY = 13,
        READONLY_MODE = 1000,
        AUTH_INVALID_TOKEN = 1001,
        AUTH_INVALID_USERNAME_PASSWORD = 1002,
        LISTENER_NOT_AUTHORIZED = 1003  // generally means a pandora one subscription expired
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
        /// If an XML parsing error occured, contains the raw XML text.
        /// </summary>
        public string XmlString {
            get { return _xmlString; }
        } protected string _xmlString = null;

        /// <summary>
        /// Creates an exception from the given server response object.
        /// </summary>
        /// <param name="response"></param>
        internal PandoraException(PandoraResponse response) {
            try {
                if (response == null) throw new PandoraException("Attempted to parse null response.");
                if (response.Success) throw new PandoraException("Attempted to parse error from successful response.");                    

                _message = response.ErrorMessage;
                _errorCode = (ErrorCodeEnum)response.ErrorCode;
            }
            catch (Exception e) {
                throw new PandoraException("Failed parsing error response.", e);
            }            
        }
        
        /// <summary>
        /// Create an exception from an error code and message provided by the Pandora servers.
        /// If the error code is recognized, the ErrorCode field will be populated.
        /// </summary>
        /// <param name="errorCodeStr"></param>
        /// <param name="message"></param>
        public PandoraException(int errorCode, string message) {
            try {
                _message = message;
                _errorCode = (ErrorCodeEnum)errorCode;
            } catch (Exception) {
                _errorCode = ErrorCodeEnum.UNKNOWN;
                _message = message;
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
        /// Creates an exception instance based on a supplied inner exception and a 
        /// message describing the situation. Should only be used for unexpected errors.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public PandoraException(string message, Exception innerException, string xmlStr) :
            base(message, innerException) {

            _message = message;
            _errorCode = ErrorCodeEnum.APPLICATION_ERROR;
            _xmlString = xmlStr;
        }

        /// <summary>
        /// Creates an exception instance with the given message describing the situation.
        /// Should only be used for unexpected errors.
        /// </summary>
        /// <param name="message"></param>
        public PandoraException(string message) :
            base(message) {

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
            this("Unexpected library error.", innerException) {

            _message = "Unexpected library error. So Sorry!";
        }

    }
}
