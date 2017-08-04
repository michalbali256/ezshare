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

    public Client()
    {
        client = new TcpClient();
        
        
    }

    public Client(TcpClient c)
    {
        this.client = c;
        stream = c.GetStream();
        ConnectInfo = new ConnectInfo(((IPEndPoint)c.Client.RemoteEndPoint).Address.GetAddressBytes(), ((IPEndPoint)c.Client.RemoteEndPoint).Port);
    }

    public async Task ConnectAsync(ConnectInfo info)
    {
        ConnectInfo = new ConnectInfo(info.IP, info.Port);
        await client.ConnectAsync(new IPAddress(info.IP), info.Port);
        stream = client.GetStream();
    }

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

    enum eMessage
    {
        Part,

    }

    

    public async Task<byte[]> ReadBytes(int count)
    {
        byte[] bytes = new byte[count];
        int rd = 0;
        while (rd < count)
        {
            int now = await stream.ReadAsync(bytes, rd, count - rd);
            rd += now;
            if (now == 0)
                throw new InvalidOperationException("Connection ended.");
        }
        return bytes;
    }

    public async Task<long> ReadLong()
    {
        byte[] buffer = new byte[8];
        await stream.ReadAsync(buffer, 0, 8);
        return BitConverter.ToInt64(buffer, 0);
    }

    public async Task SendByte(byte b)
    {
        await stream.WriteAsync(BitConverter.GetBytes(b), 0, 1);
    }
    public async Task SendInt(int integer)
    {
        await stream.WriteAsync(BitConverter.GetBytes(integer), 0, 4);
    }
    public async Task SendLong(long integer)
    {
        await stream.WriteAsync(BitConverter.GetBytes(integer), 0, 8);
    }

    internal async Task Listen(Torrent torrent)
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
                        await SendByte((byte)eRequestPartResponse.NeverAvailable);
                        break;
                    }

                    Logger.WriteLine("Listener: Request for part accepted.");
                    long part = await ReadLong();
                    Logger.WriteLine("Listener: Part number:" + part);
                    if (torrent.File.PartStatus[part] != PartFile.ePartStatus.Available)
                    {
                        Logger.WriteLine("Listener: Part " + part + " not available, sending NotAvailable flag.");
                        await SendByte((byte)eRequestPartResponse.NotAvailable);
                        break;
                    }
                    Logger.WriteLine("Listener: Sending OK response for part: " + part);
                    await SendByte((byte)eRequestPartResponse.OK);

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

    internal async Task<byte> ReadByteAsync()
    {
        byte[] buffer = new byte[1];
        await stream.ReadAsync(buffer, 0, 1);
        return buffer[0];
    }

    internal void Close()
    {
        stream.Close(100);
    }

    public async Task<string> ReceiveIdAsync()
    {
        byte[] buffer = new byte[32];
        await stream.ReadAsync(buffer, 0, 32);

        StringBuilder b = new StringBuilder();
        for (int i = 0; i < 32; ++i)
            b.Append((char)buffer[i]);
        return b.ToString();
    }

    public async Task<int> ReadInt()
    {
        byte[] buffer = new byte[4];
        await stream.ReadAsync(buffer, 0, 4);
        return BitConverter.ToInt32(buffer, 0);
    }

	public virtual void SendTorrentInfo()
	{
		throw new System.NotImplementedException();
	}

	public async Task SendBytesAsync(byte[] buffer)
	{
        await stream.WriteAsync(buffer, 0, buffer.Length);
	}

    public enum eRequestPartResponse
    {
        NotAvailable,
        OK,
        NeverAvailable        
    }

    public async Task<eRequestPartResponse> DownloadPart(PartFile file, long part)
    {
        Logger.WriteLine("Sending request for part");

        await SendByte((byte)eMessage.Part);

        Logger.WriteLine("Sending part number: " + part);
        byte[] sendPart = BitConverter.GetBytes(part);
        await stream.WriteAsync(sendPart, 0, sendPart.Length);

        int rdResponse = 0;
        rdResponse = await stream.ReadAsync(sendPart, 0, 1);
        if (rdResponse == 0)
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
        if (count > 1024 * 1024 || count < 0)
            throw new Exception();
        byte[] buffer = new byte[count];
        Logger.WriteLine("Reading content content of part:" + part);

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
        

        Logger.WriteLine("Writing part " + part + "of file " + file.FileName);
        await file.WritePartAsync(buffer, part);

        

        return eRequestPartResponse.OK;
    }

	public virtual void RequestTorrentInfo()
	{
		throw new System.NotImplementedException();
	}

	public virtual void RequestFileInfo()
	{
		throw new System.NotImplementedException();
	}

	public virtual void RequestPart()
	{
		throw new System.NotImplementedException();
	}

}

