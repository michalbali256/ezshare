using System;
using System.IO;

namespace EzShare
{
    namespace ModelLib
    {
        /// <summary>
        /// Provides Log for whole application.
        /// </summary>
        public static class Logger
        {
            private static StreamWriter sw = null;
            private static bool closed = false;


            public static event Action<string> WroteLine;


            public static void WriteLine(string line)
            {
                if (closed)
                    return;
                WroteLine?.Invoke(line);

                sw?.WriteLine(line);
            }

            public static void Close()
            {
                sw.Close();
                closed = true;
            }

            public static void Initialise(string logName)
            {
                sw = new StreamWriter(logName);
            }
        }

       
    }
}

