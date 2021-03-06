﻿using System;
using System.Runtime.Serialization;

namespace FeedServiceSDK.Exceptions
{
    [Serializable]
    public class CreateCollectionException : Exception
    {
        public CreateCollectionException()
        {
        }

        public CreateCollectionException(string message) : base(message)
        {
        }

        public CreateCollectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CreateCollectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}