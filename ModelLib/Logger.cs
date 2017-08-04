using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Logger
{
    public static event Action<string> WriteLineE;
    public static void WriteLine(string line)
    {
        WriteLineE?.Invoke(line);
    }
}

