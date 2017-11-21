using System;

namespace EzShare
{
    namespace ModelLib
    {
        /// <summary>
        /// Exception thrown when file is different than expected.
        /// </summary>
        public class WrongFileException : Exception
        {
            public WrongFileException(string message) : base(message)
            {
            }
        }
    }
}