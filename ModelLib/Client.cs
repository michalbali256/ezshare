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
        
        /*byte[] buffer = new byte[1000];
        str.Read(buffer, 0, 100);
        StringBuilder b = new StringBuilder();
        for (int i = 0; i < 100; ++i)
            b.Append((char)buffer[i]);*/
    }

    public async Task ConnectAsync(ConnectInfo info)
    {
        await client.ConnectAsync(new IPAddress(info.IP), info.Port);
        stream = client.GetStream();
    }

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

    internal async Task Listen(PartFile file)
    {
        for (;;)
        {
            byte[] b = new byte[1];
            Logger.WriteLine("Client listening for requests.");
            await stream.ReadAsync(b, 0, 1);
            switch ((eMessage)b[0])
            {
                case eMessage.Part:
                    Logger.WriteLine("Request for part accepted");
                    long part = await ReadLong();
                    if (file.PartStatus[part] != PartFile.ePartStatus.Available)
                    {
                        Logger.WriteLine("Part not available, sending NotAvailable flag.");
                        await SendByte((byte)eRequestPartResponse.NotAvailable);
                        break;
                    }
                    Logger.WriteLine("Sending OK response");
                    await SendByte((byte)eRequestPartResponse.OK);

                    byte[] buffer = new byte[file.GetPartLength(part)];
                    Logger.WriteLine("Reading part from disc");
                    await file.ReadPart(buffer, part);
                    Logger.WriteLine("Sending Part");
                    await SendBytesAsync(buffer);

                    break;
                default:
                    throw new Exception("Wrong request");
                    
            }
        }
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


    public virtual void SendFileInfo()
	{
		throw new System.NotImplementedException();

        TcpClient cl = new TcpClient("192.168.1.100", 9696);
        var ns = cl.GetStream();
        string s = "toto je velmi dlha sprava ktoru by som si chcel poslad to svojho pocitaca mala by mat 100 znakov alen neviem ze ake dlhe toto je bal babnasd";

        byte[] buffer = new byte[1000];
        for (int i = 0; i < 100; ++i)
            buffer[i] = (byte)s[i];

        ns.Write(buffer, 0, 100);
        ns.Flush();
        ns.Close();
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
        OK
    }

    public async Task<eRequestPartResponse> DownloadPart(PartFile file, long part)
    {
        Logger.WriteLine("Sending request for part");

        await SendByte((byte)eMessage.Part);

        Logger.WriteLine("Sending part number: " + part);
        byte[] sendPart = BitConverter.GetBytes(part);
        await stream.WriteAsync(sendPart, 0, sendPart.Length);

        await stream.ReadAsync(sendPart, 0, 1);

        eRequestPartResponse response = (eRequestPartResponse)Enum.Parse(typeof(eRequestPartResponse), sendPart[0].ToString());
        if (response == eRequestPartResponse.NotAvailable)
            return response;

        int count = file.GetPartLength(part);
        if (count > 1024 * 1024 || count < 0)
            throw new Exception();
        byte[] buffer = new byte[count];
        Logger.WriteLine("Reading content content of part:" + part);

        int c = 0;
        int rd = 0;
        while (rd < count)
        {
            rd += await stream.ReadAsync(buffer, rd, count - rd);
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

