using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            static StreamWriter sw = null;
            static bool closed = false;
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

