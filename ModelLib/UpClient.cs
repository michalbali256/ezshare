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
                SimpleClient = c;
                Stream = c.GetStream();
                ConnectInfo = new ConnectInfo(((IPEndPoint)c.Client.RemoteEndPoint).Address.GetAddressBytes(), ((IPEndPoint)c.Client.RemoteEndPoint).Port);
            }

            /// <summary>
            /// Endless loop listening for requests from other side.
            /// </summary>
            /// <param name="torrent">Torrent, to which is this client assigned.</param>
            /// <returns></returns>
            public async Task ListenAsync(Torrent torrent)
            {
                for (;;)
                {
                    byte[] b = new byte[1];
                    Logger.WriteLine("Listener: Client listening for requests.");
                    try
                    {
                        await Stream.ReadAsync(b, 0, 1);
                        switch ((EMessage)b[0])
                        {
                            case EMessage.Part:
                                await PartRequestAsync(torrent);
                                break;
                            case EMessage.Closing:
                                Logger.WriteLine("Other side is closing connection. Closing too.");
                                Close();
                                return;
                            default:
                                Logger.WriteLine("Bad request received.");
                                break;
                        }
                    }
                    catch (IOException exception)
                    {
                        Logger.WriteLine("Closing connection, exception thrown:" + exception);
                        Close();
                        return;
                    }
                }
            }

            private async Task PartRequestAsync(Torrent torrent)
            {
                if (torrent.Status != Torrent.EStatus.Seeding && torrent.Status != Torrent.EStatus.Downloading)
                {
                    Logger.WriteLine("Listener: Torrent is paused or stopped or error, sending NeverAvailable flag.");
                    await SendByteAsync((byte)ERequestPartResponse.NeverAvailable);
                    return;
                }

                Logger.WriteLine("Listener: Request for part accepted.");
                long part = await ReadLongAsync();
                Logger.WriteLine("Listener: Part number:" + part);
                if (torrent.File.PartStatus[part] != PartFile.EPartStatus.Available)
                {
                    Logger.WriteLine("Listener: Part " + part + " not available, sending NotAvailable flag.");
                    await SendByteAsync((byte)ERequestPartResponse.NotAvailable);
                    return;
                }
                Logger.WriteLine("Listener: Sending OK response for part: " + part);
                await SendByteAsync((byte)ERequestPartResponse.OK);

                byte[] buffer = new byte[torrent.File.GetPartLength(part)];
                Logger.WriteLine("Listener: Reading part " + part + " from disc");

                await torrent.File.ReadPartAsync(buffer, part);
                Logger.WriteLine("Listener: Sending Part" + part);
                await SendBytesAsync(buffer);
            }
        }
    }
}