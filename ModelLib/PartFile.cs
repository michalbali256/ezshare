using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Security.Cryptography;

public class WrongFileException : Exception
    {
        public WrongFileException(string message) : base(message) { }
    }

public class PartFile : IDisposable
{
    public int PartSize = 1024*1024;

    public PartFile(string filePath)
    {
        FilePath = filePath;
        openFile(filePath);
    }

    public enum ePartStatus
    {
        Missing,
        Processing,
        Available
    }

    public FileStream stream;

    public string Hash { get; private set; }
    public long NumberOfParts { get; set; }
    public ePartStatus[] PartStatus;
    public string FilePath
    {
        get;
        private set;
    }
    public string FileName
    {
        get;
        private set;
    }
    public long Size
    {
        get;
        private set;
    }
    public long Progress
    {
        get
        {
            long ava = 0;
            for (int i = 0; i < NumberOfParts; ++i)
                if (PartStatus[i] == ePartStatus.Available)
                    ++ava;

            return ava;
        }
    }
    Random rnd = new Random();

    private void openFile(string filePath)
    {
        
        stream = File.Open(filePath, FileMode.OpenOrCreate);

        var split = filePath.Split('\\');
        FileName = split[split.Length - 1];
        Size = stream.Length;

        NumberOfParts = (Size - 1) / PartSize + 1;
        PartStatus = new ePartStatus[NumberOfParts];

        
        Hash = hashFromStream(stream);
    }

    public void Seek(long part)
    {
        stream.Seek(part * PartSize, SeekOrigin.Begin);
    }
    public int GetPartLength(long part)
    {
        if (part == NumberOfParts - 1)
            return (int)(Size % PartSize);
        else
            return PartSize;
    }

    private string hashFromStream(FileStream str)
    {
        var md5 = MD5.Create();

        return hashToString(md5.ComputeHash(str));
    }

    private static string hashToString(byte[] hash)
    {
        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }

    public const string XmlName = "partfile";
    public XmlElement SaveToXml(XmlDocument doc)
    {
        XmlElement elem = doc.CreateElement(XmlName);
        elem.AppendElementWithValue("filepath", FilePath);
        elem.AppendElementWithValue("numberofparts", NumberOfParts.ToString());
        elem.AppendElementWithValue("filename", FileName);
        elem.AppendElementWithValue("hash", Hash);
        elem.AppendElementWithValue("size", Size.ToString());
        

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < NumberOfParts; ++i)
            sb.Append((int)PartStatus[i]);

        elem.AppendElementWithValue("partstatus", sb.ToString());

        return elem;
    }

    internal async Task ReadPart(byte[] buffer, long part)
    {
        Seek(part);
        await stream.ReadAsync(buffer, 0, GetPartLength(part));
    }

    public static PartFile FromXml(XmlElement elem, bool checkHash)
    {
        PartFile file = new PartFile(elem["filepath"].InnerText);

        if (checkHash && file.Hash != elem["hash"].InnerText)
            throw new WrongFileException("File has changed");

        file.FileName = elem["filename"].InnerText;
        file.NumberOfParts = long.Parse(elem["numberofparts"].InnerText);
        file.PartStatus = new ePartStatus[file.NumberOfParts];
        string partstatus = elem["partstatus"].InnerText;
        
        for (int i = 0; i < partstatus.Length; ++i)
        {
            file.PartStatus[i] = (ePartStatus) Enum.Parse(typeof(ePartStatus), partstatus[i].ToString());
        }

        

        return file;
    }

    internal void Close()
    {
        stream.Flush();
        stream.Close();
    }

    internal async Task WritePartAsync(byte[] buffer, long part)
    {
        Seek(part);
        await stream.WriteAsync(buffer, 0, GetPartLength(part));
    }

    public XmlElement SaveToXmlShare(XmlDocument doc)
    {
        XmlElement elem = doc.CreateElement(XmlName);
        elem.AppendElementWithValue("numberofparts", NumberOfParts.ToString());
        elem.AppendElementWithValue("filename", FileName);
        elem.AppendElementWithValue("hash", Hash);
        elem.AppendElementWithValue("size", Size.ToString());

        return elem;
    }

    public static PartFile FromXmlShare(XmlElement elem, string filePath)
    {
        PartFile file = new PartFile(filePath);
        
        file.NumberOfParts = int.Parse(elem["numberofparts"].InnerText);

        file.PartStatus = new ePartStatus[file.NumberOfParts];

        file.FileName = elem["filename"].InnerText;
        file.Hash = elem["hash"].InnerText;
        file.Size = long.Parse(elem["size"].InnerText);

        file.setLength(file.Size);

        return file;
    }

    public static PartFile FromPath(string filePath)
    {
        PartFile file = new PartFile(filePath);

        for(int i = 0; i < file.PartStatus.Length; ++i)
            file.PartStatus[i] = ePartStatus.Available;

        return file;
    }




    private void setLength(long length)
    {
        stream.SetLength(length);
    }

    public void Dispose()
    {
        stream.Dispose();
    }

    public long GetPartIndex(ePartStatus status)
    {
        int count = 0;
        long i;

        do
        {
            ++count;
            i = longRandom(NumberOfParts);
        } while (PartStatus[i] != status && count < 30);

        if (PartStatus[i] == status)
            return i;

        for (i = 0; i < NumberOfParts; ++i)
            if (PartStatus[i] == status)
                return i;

        return -1;
    }

    long longRandom(long max)
    {
        return longRandom(0, max);
    }

    long longRandom(long min, long max)
    {
        byte[] buf = new byte[8];
        rnd.NextBytes(buf);
        long longRand = BitConverter.ToInt64(buf, 0);

        return (Math.Abs(longRand % (max - min)) + min);
    }
}

