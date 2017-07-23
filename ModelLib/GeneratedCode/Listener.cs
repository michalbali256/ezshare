
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Listener
{
}

/*Thread t = new Thread(() =>
            {
                TcpListener lis = new TcpListener(new IPAddress(new byte[] { 192, 168, 1, 100 }), 9696);
                lis.Start();
                TcpClient cl;
                while (true)
                {
                    cl = lis.AcceptTcpClient();
                    NetworkStream str = cl.GetStream();
                    byte[] buffer = new byte[1000];
                    str.Read(buffer, 0, 100);
                    StringBuilder b = new StringBuilder();
                    for (int i = 0; i < 100; ++i)
                        b.Append((char)buffer[i]);
                    MessageBox.Show(b.ToString());
                }
            });

            t.Start();*/
