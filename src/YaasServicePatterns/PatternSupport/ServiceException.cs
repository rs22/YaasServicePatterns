using System;
using System.Net;

namespace YaasServicePatterns.PatternSupport
{
    public class ServiceException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public ServiceException(HttpStatusCode statusCode, string content) : base(content)
        {
            StatusCode = statusCode;
        }
    }
}
    