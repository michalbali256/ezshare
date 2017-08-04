using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public static class Logger
{
    public static StreamWriter sw = new StreamWriter("log.log");

    public static event Action<string> OnWriteLine;
    public static void WriteLine(string line)
    {
        OnWriteLine?.Invoke(line);
        sw.WriteLine(line);
    }
}

