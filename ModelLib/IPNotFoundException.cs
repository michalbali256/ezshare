using System;

namespace EzShare
{
    namespace ModelLib
    {
        /// <summary>
        /// Represents exception that this device has no IP (probably not connected to internet).
        /// </summary>
        class IPNotFoundException : Exception
        {
            public IPNotFoundException(string message) : base(message)
            {

            }
        }
    }
}