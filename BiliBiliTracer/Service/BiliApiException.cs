using System;

namespace BiliBiliTracer.Service
{
    class BiliApiException : Exception
    {
        public BiliApiException(string message) : base(message) { }
    }
}
