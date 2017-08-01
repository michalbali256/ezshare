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

public class PartFile
{

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

    BinaryReader fileReader;
    BinaryWriter fileWriter;

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


    private void openFile(string filePath)
    {
        var stream = File.Open(filePath, FileMode.OpenOrCreate);

        fileReader = new BinaryReader(stream);
        fileWriter = new BinaryWriter(stream);

        NumberOfParts = (stream.Length - 1) / 1024 + 1;
        PartStatus = new ePartStatus[NumberOfParts];

        using (var md5 = MD5.Create())
        {
            using (var str = File.OpenRead(FilePath))
            {
                Hash = hashToString(md5.ComputeHash(str));
            }
        }

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
        elem.AppendElementWithValue("filepath", FilePath);
        elem.AppendElementWithValue("hash", Hash);
        elem.AppendElementWithValue("size", Size.ToString());
        

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < NumberOfParts; ++i)
            sb.Append((int)PartStatus[i]);

        elem.AppendElementWithValue("partstatus", sb.ToString());

        return elem;
    }

    public static PartFile FromXml(XmlElement elem)
    {
        PartFile file = new PartFile(elem["filepath"].InnerText);

        if (file.NumberOfParts != int.Parse(elem["numberofparts"].InnerText))
            throw new WrongFileException("File has changed");

        t.Hash = elem["hash"].InnerText;

        var split = t.FilePath.Split('\\');
        t.FileName = split[split.Length - 1];


        return file;
    }

}

