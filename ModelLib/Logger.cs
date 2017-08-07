using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public static class Logger
{
    static StreamWriter sw = new StreamWriter("log.log");
    static bool closed = false;
    public static event Action<string> WroteLine;
    public static void WriteLine(string line)
    {
        if (closed)
            return;
        WroteLine?.Invoke(line);
        
        sw.WriteLine(line);
    }

    public static void Close()
    {
        sw.Close();
        closed = true;
    }
}

