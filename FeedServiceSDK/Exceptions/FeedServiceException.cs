using System;
using System.Runtime.Serialization;

namespace FeedServiceSDK.Exceptions
{
    [Serializable]
    public class FeedServiceException : Exception
    {
        public FeedServiceException()
        {
        }

        public FeedServiceException(string message) : base(message)
        {
        }

        public FeedServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FeedServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}