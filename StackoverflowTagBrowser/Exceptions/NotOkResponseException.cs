using System.Net;

namespace StackoverflowTagBrowser.Exceptions
{
    public class NotOkResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public NotOkResponseException(string message, HttpStatusCode statusCode) : base(message) 
        {
            StatusCode = statusCode;
        }
    }
}
