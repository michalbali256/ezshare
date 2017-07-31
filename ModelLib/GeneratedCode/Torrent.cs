﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Threading.Tasks;

public class Torrent
{
    public Torrent()
    {
        id = "";
        Random r = new Random();
        byte[] bytes = new byte[16];
        for (int i = 0; i < 16; ++i)
            bytes[i] = (byte)r.Next(256);
        id = hashToString(bytes);
    }

    public Torrent(string id)
    {
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
		get;
		private set;
	}
    public virtual string FileName
    {
        get;
        private set;
    }


    public virtual object ProgressOfFile
	{
		get;
	}

	public virtual List<Client> Clients
	{
		get;
		set;
	}

    public long Size
    {
        get;
        private set;
    }
    public string Hash { get; private set; }
    public string Name { get; set; }
    public static string XmlName = "torrent";

    public XmlElement SaveToXml(XmlDocument doc)
    {
        XmlElement e = doc.CreateElement(XmlName);

        e.AppendChild(CreateElementWithValue(doc, "name", Name));
        e.AppendChild(CreateElementWithValue(doc, "id", Id));
        e.AppendChild(CreateElementWithValue(doc, "filename", FileName));
        e.AppendChild(CreateElementWithValue(doc, "filepath", FilePath));
        e.AppendChild(CreateElementWithValue(doc, "hash", Hash));
        e.AppendChild(CreateElementWithValue(doc, "size", Size.ToString()));
        e.AppendChild(CreateElementWithValue(doc, "status", Status.ToString()));

        return e;
    }

    internal Task Download()
    {
        Status = Torrent.eStatus.Downloading;
        throw new NotImplementedException();
    }

    private XmlElement CreateElementWithValue(XmlDocument doc, string xmlName, string value)
    {
        XmlElement elem = doc.CreateElement(xmlName);
        elem.InnerText = value;
        return elem;
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
        t.FilePath = path;
        var split = t.FilePath.Split('\\');
        t.FileName = split[split.Length - 1];
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(t.FilePath))
            {
                t.Hash = hashToString(md5.ComputeHash(stream));
            }
        }

        var fi = new FileInfo(path);
        t.Size = fi.Length;
        t.Name = t.FileName;
        t.Status = eStatus.Seeding;
        return t;
    }

    public static Torrent CreateFromXml(XmlElement elem)
    {
        Torrent t = new Torrent(elem["id"].InnerText);
        t.Name = elem["name"].InnerText;
        t.FileName = elem["filename"].InnerText;
        t.FilePath = elem["filepath"].InnerText;
        t.Hash = elem["hash"].InnerText;
        t.Size = int.Parse(elem["size"].InnerText);
        t.Status = (eStatus) Enum.Parse(typeof(eStatus), elem["status"].InnerText);
        return t;
    }

}

