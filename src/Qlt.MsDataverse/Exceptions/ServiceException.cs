using System;
using System.Collections.Generic;
using System.Text;

namespace Qlt.MsDataverse.Exceptions
{
    /// <summary>
    /// An exception that captures data returned by the Web API
    /// </summary>
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class ServiceException : Exception
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
    {
        public int ErrorCode { get; private set; }
        public int StatusCode { get; private set; }
        public string ReasonPhrase { get; private set; }

        public ServiceException()
        {
        }

        public ServiceException(string message) : base(message)
        {
        }

        public ServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ServiceException(int errorcode, int statuscode, string reasonphrase, string message) : base(message)
        {
            ErrorCode = errorcode;
            StatusCode = statuscode;
            ReasonPhrase = reasonphrase;
        }
    }
}
