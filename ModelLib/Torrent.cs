﻿using System;
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
        Seeding
    }

    internal void AddClient(Client mc)
    {
        Clients.Add(mc);
        mc.Listen(File);
    }

    

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
		get { return File.Progress; }
	}

	public virtual List<Client> Clients
	{
		get;
		set;
	}

    public PartFile File;
    public long Size
    {
        get { return File.Size; }
    }
    
    public string Name { get; set; }
    public long NumberOfParts { get { return File.NumberOfParts; } }

    public static string XmlName = "torrent";

    public XmlElement SaveToXml(XmlDocument doc)
    {
        XmlElement e = doc.CreateElement(XmlName);

        e.AppendElementWithValue("name", Name);
        e.AppendElementWithValue("id", Id);
        e.AppendElementWithValue("status", Status.ToString());
        e.AppendChild(File.SaveToXml(doc));

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

    internal async Task Download()
    {
        
        Status = Torrent.eStatus.Downloading;
        Logger.WriteLine("Starting download:" + Name);
        long part;
        var tasks = new Dictionary<Task<Client.eRequestPartResponse>, Tuple<Client, long>>();
        foreach (var c in Clients)
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

            Client clientEnded = tasks[t].Item1;
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
            if (part != -1)
            {
                File.PartStatus[part] = PartFile.ePartStatus.Processing;
                tasks.Add(clientEnded.DownloadPart(File, part), Tuple.Create(clientEnded, part));
            }
        }
        Status = Torrent.eStatus.Seeding;
    }

    internal void Close()
    {
        File.Close();
    }

    private static string hashToString(byte[] hash)
    {
        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }


	public virtual void Start()
	{
		throw new System.NotImplementedException();
	}

	public virtual void Pause()
	{
		throw new System.NotImplementedException();
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

        t.File = PartFile.FromXml(elem[PartFile.XmlName], t.Status == eStatus.Seeding); //check hash only if the the file is already downloaded - otherwise the hash must be different

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



