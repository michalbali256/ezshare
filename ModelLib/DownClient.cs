using System;
using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace EzShare
{
    namespace ModelLib
    {
        /// <summary>
        /// Provides Client, that can download parts of file
        /// </summary>
        public class DownClient : Client
        {
            /// <summary>
            /// Constructs new downloading Client that can be connected afterwards.
            /// </summary>
            public DownClient()
            {
                SimpleClient = new TcpClient();
            }

            /// <summary>
            /// Connects Client using ConnectInfo (IP and port)
            /// </summary>
            /// <param name="info"></param>
            /// <returns></returns>
            public async Task ConnectAsync(ConnectInfo info)
            {
                ConnectInfo = new ConnectInfo(info.IP, info.Port);
                await SimpleClient.ConnectAsync(new IPAddress(info.IP), info.Port);
                Stream = SimpleClient.GetStream();
            }

            /// <summary>
            /// Downloads one part from connected client and writes it into specified file
            /// </summary>
            /// <param name="file">PartFile into which will be the part written</param>
            /// <param name="part">Number of downloaded part</param>
            /// <returns>Returns eRequestPartResponse, which indicates whether process went well or the part was not available</returns>
            public async Task<ERequestPartResponse> DownloadPartAsync(PartFile file, long part)
            {
                try
                {
                    Logger.WriteLine("Sending request for part");

                    await SendByteAsync((byte)EMessage.Part);

                    Logger.WriteLine("Sending part number: " + part);

                    await SendLongAsync(part);

                    byte resp = await ReadByteAsync();

                    ERequestPartResponse response = (ERequestPartResponse)Enum.Parse(typeof(ERequestPartResponse), resp.ToString());
                    if (response != ERequestPartResponse.OK)
                    {
                        Logger.WriteLine("Part " + response.ToString());
                        return response;
                    }
                    int count = file.GetPartLength(part);

                    Logger.WriteLine("Reading content content of part:" + part);
                    byte[] buffer = await ReadBytesAsync(count);

                    //writes part into the file
                    Logger.WriteLine("Writing part " + part + "of file " + file.FileName);
                    await file.WritePartAsync(buffer, part);


                    return ERequestPartResponse.OK;
                }
                catch (IOException exception)
                {
                    Logger.WriteLine("Closing connection. Exception caught: " + exception.Message);
                    Close();
                    return ERequestPartResponse.NeverAvailable;
                }
                catch (SocketException exception)
                {
                    Logger.WriteLine("Closing connection. Exception caught: " + exception.Message);
                    Close();
                    return ERequestPartResponse.NeverAvailable;
                }
            }

            public override async void Close()
            {
                await SendByteAsync((byte)EMessage.Closing);
                base.Close();
            }
        }
    }
}
