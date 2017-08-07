using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Threading.Tasks;

public class Torrent : IDisposable
{
    private Torrent()
    {
        Clients = new List<Client>();
        id = "";
        Random r = new Random();
        byte[] bytes = new byte[16];
        for (int i = 0; i < 16; ++i)
            bytes[i] = (byte)r.Next(256);
        id = hashToString(bytes);
    }

    private Torrent(string id)
    {
        Clients = new List<Client>();
        this.id = id;
    }

    public enum eStatus
    {
        Downloading,
        Paused,
        Stopped,
        Seeding,
        Error
    }

    public void AddClient(Client client)
    {
        client.ClientClosed += mc_ClientClosed;
        Clients.Add(client);
    }

    public void RemoveClient(Client client)
    {
        client.ClientClosed -= mc_ClientClosed;
        Clients.Remove(client);
    }

    private void mc_ClientClosed(Client sender)
    {
        Clients.Remove(sender);
        sender.ClientClosed -= mc_ClientClosed;
    }

    public HashSet<ConnectInfo> ClientsInfo { get; set; } = new HashSet<ConnectInfo>();

    public virtual eStatus Status
	{
		get;
		set;
	}

	public virtual object DownloadSpeed
	{
		get;
	}


    private readonly string id;
    public  string Id
    {
        get { return id;}
    }

	public virtual object UploadSpeed
	{
		get;
	}

	public virtual string FilePath
	{
		get { return File.FilePath; }
		//private set;
	}
    public virtual string FileName
    {
        get { return File.FileName; }
        //private set;
    }


    public long ProgressOfFile
	{
		get { return File == null ? 0 : File.Progress; }
	}

	public List<Client> Clients
	{
		get;
		set;
	}
    public IEnumerable<DownClient> DownClients
    {
        get
        {
            return Clients.OfType<DownClient>();
        }
    }

    public IEnumerable<UpClient> UpClients
    {
        get
        {
            return Clients.OfType<UpClient>();
        }
    }

    public PartFile File;
    public long Size
    {
        get { return File == null ? 0 : File.Size; }
    }
    
    public string Name { get; set; }
    public long NumberOfParts { get { return File == null ? 0 : File.NumberOfParts; } }

    public static string XmlName = "torrent";

    public XmlElement SaveToXml(XmlDocument doc)
    {
        XmlElement e = doc.CreateElement(XmlName);

        e.AppendElementWithValue("name", Name);
        e.AppendElementWithValue("id", Id);
        e.AppendElementWithValue("status", Status.ToString());
        if(Status != eStatus.Error)
            e.AppendChild(File.SaveToXml(doc));

        XmlElement clients = doc.CreateElement("clients");
        foreach (ConnectInfo info in this.ClientsInfo)
            clients.AppendChild(info.SaveToXml(doc));
        e.AppendChild(clients);
        return e;
    }

    public XmlElement SaveToXmlShare(XmlDocument doc)
    {
        XmlElement e = doc.CreateElement(XmlName);

        e.AppendElementWithValue("name", Name);
        e.AppendElementWithValue("id", Id);
        e.AppendChild(File.SaveToXmlShare(doc));

        return e;
    }

    

    bool downloadIsRunning = false;
    public async Task Download()
    {
        
        downloadIsRunning = true;
        Status = Torrent.eStatus.Downloading;
        Logger.WriteLine("Starting download:" + Name);
        long part;
        var tasks = new Dictionary<Task<Client.eRequestPartResponse>, Tuple<DownClient, long>>();
        foreach (var c in DownClients)
        {
            part = File.GetPartIndex(PartFile.ePartStatus.Missing);
            File.PartStatus[part] = PartFile.ePartStatus.Processing;
            if (part == -1)
                break;
            tasks.Add(c.DownloadPart(File, part), Tuple.Create(c, part));
        }

        while(tasks.Count != 0)
        {
            var t = await Task.WhenAny(tasks.Keys);

            DownClient clientEnded = tasks[t].Item1;
            long partEnded = tasks[t].Item2;

            if (t.Result == Client.eRequestPartResponse.OK)
            {
                File.PartStatus[partEnded] = PartFile.ePartStatus.Available;
                Logger.WriteLine("Part transfered successfuly.");
            }
            else
                File.PartStatus[partEnded] = PartFile.ePartStatus.Missing;
            tasks.Remove(t);

            part = File.GetPartIndex(PartFile.ePartStatus.Missing);
            if (part != -1 && Status == eStatus.Downloading && Clients.Contains(clientEnded))
            {
                File.PartStatus[part] = PartFile.ePartStatus.Processing;
                tasks.Add(clientEnded.DownloadPart(File, part), Tuple.Create(clientEnded, part));
            }
            
        }
        if(Status == eStatus.Downloading && ProgressOfFile == NumberOfParts)
            Status = Torrent.eStatus.Seeding;
        Logger.WriteLine("Ending download." + Name);
        downloadIsRunning = false;
    }

    public void Pause()
    {
        Status = eStatus.Paused;
    }

    public async void Start()
    {
        if (Status == eStatus.Error)
        {
            Logger.WriteLine("Cannot start torrent with error");
            return;
        }

        if (ProgressOfFile == NumberOfParts)
        {
            Status = eStatus.Seeding;
        }
        else
        {
            Status = eStatus.Downloading;
            if (!downloadIsRunning)
                await Download();
        }

    }

    internal void Close()
    {
        File?.Close();
    }

    private static string hashToString(byte[] hash)
    {
        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }

    public static Torrent CreateFromPath(string path)
    {
        Torrent t = new Torrent();
        t.File = PartFile.FromPath(path);
        
        t.Name = t.FileName;
        t.Status = eStatus.Seeding;
        return t;
    }

    public static Torrent CreateFromXml(XmlElement elem)
    {
        Torrent t = new Torrent(elem["id"].InnerText);
        t.Name = elem["name"].InnerText;
        t.Status = (eStatus)Enum.Parse(typeof(eStatus), elem["status"].InnerText);
        if(t.Status != eStatus.Error)
            try
            {
                t.File = PartFile.FromXml(elem[PartFile.XmlName], t.Status == eStatus.Seeding); //check hash only if the the file is already downloaded - otherwise the hash must be different
            }
            catch (WrongFileException)
            {
                t.Status = eStatus.Error;
            }
            catch (IOException)
            {
                t.Status = eStatus.Error;
            }
        foreach (XmlElement e in elem["clients"])
        {
            t.ClientsInfo.Add(ConnectInfo.ParseXml(e));
        }

        return t;
    }

    public static Torrent CreateFromXmlShare(XmlElement elem, string filePath)
    {
        Torrent t = new Torrent(elem["id"].InnerText);
        t.Name = elem["name"].InnerText;

        t.File = PartFile.FromXmlShare(elem[PartFile.XmlName], filePath);
        
        return t;
    }

    public void Dispose()
    {
        File.Dispose();
    }
}



