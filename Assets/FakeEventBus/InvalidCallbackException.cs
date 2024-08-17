using System;

namespace FakeEventBus
{
    public class InvalidCallbackException : Exception
    {
        public InvalidCallbackException(string message) : base(message) { }
    }
}
