using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace EzShare
{
    namespace ModelLib
    {
        /// <summary>
        /// Provides Client able to upload parts of file
        /// </summary>
        public class UpClient : Client
        {
            /// <summary>
            /// Creates new listening Client from existing TcpClient
            /// </summary>
            /// <param name="c">TcpClient that should be connected.</param>
            public UpClient(TcpClient c)
            {
                this.client = c;
                stream = c.GetStream();
                ConnectInfo = new ConnectInfo(((IPEndPoint)c.Client.RemoteEndPoint).Address.GetAddressBytes(), ((IPEndPoint)c.Client.RemoteEndPoint).Port);
            }

            /// <summary>
            /// Endless loop listening for requests from other side.
            /// </summary>
            /// <param name="torrent">Torrent, to which is this client assigned.</param>
            /// <returns></returns>
            internal async Task ListenAsync(Torrent torrent)
            {
                for (;;)
                {
                    byte[] b = new byte[1];
                    Logger.WriteLine("Listener: Client listening for requests.");
                    try
                    {
                        await stream.ReadAsync(b, 0, 1);
                        switch ((eMessage)b[0])
                        {
                            case eMessage.Part:
                                if (torrent.Status != Torrent.eStatus.Seeding && torrent.Status != Torrent.eStatus.Downloading)
                                {
                                    Logger.WriteLine("Listener: Torrent is paused or stopped or error, sending NeverAvailable flag.");
                                    await SendByteAsync((byte)eRequestPartResponse.NeverAvailable);
                                    break;
                                }

                                Logger.WriteLine("Listener: Request for part accepted.");
                                long part = await ReadLongAsync();
                                Logger.WriteLine("Listener: Part number:" + part);
                                if (torrent.File.PartStatus[part] != PartFile.ePartStatus.Available)
                                {
                                    Logger.WriteLine("Listener: Part " + part + " not available, sending NotAvailable flag.");
                                    await SendByteAsync((byte)eRequestPartResponse.NotAvailable);
                                    break;
                                }
                                Logger.WriteLine("Listener: Sending OK response for part: " + part);
                                await SendByteAsync((byte)eRequestPartResponse.OK);

                                byte[] buffer = new byte[torrent.File.GetPartLength(part)];
                                Logger.WriteLine("Listener: Reading part " + part + " from disc");

                                await torrent.File.ReadPart(buffer, part);
                                Logger.WriteLine("Listener: Sending Part" + part);
                                await SendBytesAsync(buffer);

                                break;
                            case eMessage.Closing:
                                Logger.WriteLine("Other side is closing connection. Closing too.");
                                this.Close();
                                return;
                            default:
                                throw new Exception("Listener: Wrong request");
                        }
                    }
                    catch (IOException exception)
                    {
                        Logger.WriteLine("Closing connection, exception thrown:" + exception);
                        this.Close();
                        return;
                    }
                }
            }
        }
    }
}