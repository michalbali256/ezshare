using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
public class Client
{
    private TcpClient client;
    private NetworkStream stream;

    /// <summary>
    /// Constructs new Client that can be connected afterwards.
    /// </summary>
    public Client()
    {
        client = new TcpClient();
    }

    /// <summary>
    /// Creates new Client from existing TcpClient
    /// </summary>
    /// <param name="c">TcpClient that should be connected.</param>
    public Client(TcpClient c)
    {
        this.client = c;
        stream = c.GetStream();
        ConnectInfo = new ConnectInfo(((IPEndPoint)c.Client.RemoteEndPoint).Address.GetAddressBytes(), ((IPEndPoint)c.Client.RemoteEndPoint).Port);
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
    /// IP and port of remote end point.
    /// </summary>
    public ConnectInfo ConnectInfo { get; private set; }

    /// <summary>
    /// Sends id.
    /// </summary>
    /// <param name="id">The id that should be 32 chars long</param>
    public async Task SendIdAsync(string id)
    {
        byte[] buffer = new byte[32];
        for (int i = 0; i < 32; ++i)
            buffer[i] = (byte)id[i];
        await stream.WriteAsync(buffer, 0, 32);
    }

    /// <summary>
    /// Represents types of requests that can be sent
    /// </summary>
    enum eMessage
    {
        Part,

    }

    /// <summary>
    /// Asynchronously reads bytes from remote client.
    /// </summary>
    /// <param name="count">The number of bytes to expect</param>
    /// <returns></returns>
    public async Task<byte[]> ReadBytesAsync(int count)
    {
        byte[] bytes = new byte[count];
        int rd = 0;
        //reads until expected number of bytes were read - sometimes it takes more than one NetworkStream.ReadAsync to read it all.
        while (rd < count)
        {
            int now = await stream.ReadAsync(bytes, rd, count - rd);
            rd += now;
            if (now == 0)//if 0 bytes were read, it means connection is bad
                throw new InvalidOperationException("Connection ended.");
        }
        return bytes;
    }
    public async Task<long> ReadLongAsync()
    {
        return BitConverter.ToInt64(await ReadBytesAsync(8), 0);
    }
    public async Task<byte> ReadByteAsync()
    {
        byte[] buffer = new byte[1];
        await stream.ReadAsync(buffer, 0, 1);
        return buffer[0];
    }
    public async Task<int> ReadIntAsync()
    {
        return BitConverter.ToInt32(await ReadBytesAsync(4), 0);
    }
    /// <summary>
    /// Reads 32 bytes long Id from remote client
    /// </summary>
    /// <returns>Task that returns 32 bytes long string representing id</returns>
    public async Task<string> ReadIdAsync()
    {
        byte[] buffer = new byte[32];
        await stream.ReadAsync(buffer, 0, 32);

        StringBuilder b = new StringBuilder();
        for (int i = 0; i < 32; ++i)
            b.Append((char)buffer[i]);
        return b.ToString();
    }


    public async Task SendByteAsync(byte b)
    {
        await stream.WriteAsync(BitConverter.GetBytes(b), 0, 1);
    }
    public async Task SendIntAsync(int integer)
    {
        await stream.WriteAsync(BitConverter.GetBytes(integer), 0, 4);
    }
    public async Task SendLongAsync(long integer)
    {
        await stream.WriteAsync(BitConverter.GetBytes(integer), 0, 8);
    }
    public async Task SendBytesAsync(byte[] buffer)
    {
        await stream.WriteAsync(buffer, 0, buffer.Length);
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
                default:
                    throw new Exception("Listener: Wrong request");
            }
        }
    }


    /// <summary>
    /// Releases resources and closes the connection.
    /// </summary>
    internal void Close()
    {
        stream.Close(100);
    }




    public enum eRequestPartResponse
    {
        NotAvailable,
        OK,
        NeverAvailable        
    }
    /// <summary>
    /// Downloads one part from connected client and writes it into specified file
    /// </summary>
    /// <param name="file">PartFile into which will be the part written</param>
    /// <param name="part">Number of downloaded part</param>
    /// <returns>Returns eRequestPartResponse, which indicates whether process went well or the part was not available</returns>
    public async Task<eRequestPartResponse> DownloadPart(PartFile file, long part)
    {
        Logger.WriteLine("Sending request for part");

        await SendByteAsync((byte)eMessage.Part);

        Logger.WriteLine("Sending part number: " + part);
        byte[] sendPart = BitConverter.GetBytes(part);
        await stream.WriteAsync(sendPart, 0, sendPart.Length);

        int rdResponse = 0;
        rdResponse = await stream.ReadAsync(sendPart, 0, 1);
        if (rdResponse == 0)//if readASync returns 0, the connection ended.
        {
            Logger.WriteLine("Connection ended" + part);
            return eRequestPartResponse.NotAvailable;
        }
        eRequestPartResponse response = (eRequestPartResponse)Enum.Parse(typeof(eRequestPartResponse), sendPart[0].ToString());
        if (response != eRequestPartResponse.OK)
        {
            Logger.WriteLine("Part " + response.ToString());
            return response;
        }
        int count = file.GetPartLength(part);

        byte[] buffer = new byte[count];
        Logger.WriteLine("Reading content content of part:" + part);

        //reads until expected number of bytes were read - sometimes it takes more than one NetworkStream.ReadAsync to read it all.
        int c = 0;
        int rd = 0;
        while (rd < count)
        {
            int rdNow = await stream.ReadAsync(buffer, rd, count - rd);
            if (rdNow == 0)
                return eRequestPartResponse.NeverAvailable;
            rd += rdNow;
            if (++c > 1000)
                throw new Exception("BADREAD");
        }
        

        //writes part into the file
        Logger.WriteLine("Writing part " + part + "of file " + file.FileName);
        await file.WritePartAsync(buffer, part);


        return eRequestPartResponse.OK;
    }
}

