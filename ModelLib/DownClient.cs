using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                client = new TcpClient();
            }

            /// <summary>
            /// Connects Client using ConnectInfo (IP and port)
            /// </summary>
            /// <param name="info"></param>
            /// <returns></returns>
            public async Task ConnectAsync(ConnectInfo info)
            {
                ConnectInfo = new ConnectInfo(info.IP, info.Port);
                await client.ConnectAsync(new IPAddress(info.IP), info.Port);
                stream = client.GetStream();
            }

            /// <summary>
            /// Downloads one part from connected client and writes it into specified file
            /// </summary>
            /// <param name="file">PartFile into which will be the part written</param>
            /// <param name="part">Number of downloaded part</param>
            /// <returns>Returns eRequestPartResponse, which indicates whether process went well or the part was not available</returns>
            public async Task<eRequestPartResponse> DownloadPart(PartFile file, long part)
            {
                try
                {
                    Logger.WriteLine("Sending request for part");

                    await SendByteAsync((byte)eMessage.Part);

                    Logger.WriteLine("Sending part number: " + part);

                    await SendLongAsync(part);

                    byte resp = await ReadByteAsync();

                    eRequestPartResponse response = (eRequestPartResponse)Enum.Parse(typeof(eRequestPartResponse), resp.ToString());
                    if (response != eRequestPartResponse.OK)
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


                    return eRequestPartResponse.OK;
                }
                catch (IOException exception)
                {
                    Logger.WriteLine("Closing connection. Exception caught: " + exception.Message);
                    this.Close();
                    return eRequestPartResponse.NeverAvailable;
                }
                catch (SocketException exception)
                {
                    Logger.WriteLine("Closing connection. Exception caught: " + exception.Message);
                    this.Close();
                    return eRequestPartResponse.NeverAvailable;
                }
            }

            public async override void Close()
            {
                await SendByteAsync((byte)eMessage.Closing);
                base.Close();
            }
        }
    }
}
