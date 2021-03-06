﻿using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using System.Threading;

namespace EzShare
{
    namespace ModelLib
    {
        /// <summary>
        /// Represents file, that is splitted into parts in order to share it.
        /// </summary>
        public class PartFile : IDisposable
        {
            public const string XmlName = "partfile";

            /// <summary>
            /// Size of one part in bytes.
            /// </summary>
            const int PartSize = 1024 * 1024;

            /// <summary>
            /// Underlying handle of file.
            /// </summary>
            private FileStream stream;

            private Random rnd = new Random();
            private SemaphoreSlim writeSemaphore = new SemaphoreSlim(1, 1);

            /// <summary>
            /// Opens specified file.
            /// </summary>
            /// <param name="filePath">Path to the file.</param>
            public PartFile(string filePath)
            {
                FilePath = filePath;
                OpenFile(filePath);
            }

            /// <summary>
            /// Represents status of one part
            /// </summary>
            public enum EPartStatus
            {
                Missing,
                Processing,
                Available
            }
            

            /// <summary>
            /// Hash of the final file that is being shared
            /// </summary>
            public string Hash { get; private set; }
            /// <summary>
            /// Total Number of parts into which is file separated
            /// </summary>
            public long NumberOfParts { get; private set; }

            /// <summary>
            /// Status of every part.
            /// </summary>
            public EPartStatus[] PartStatus { get; private set; }

            /// <summary>
            /// Path to the underlying file
            /// </summary>
            public string FilePath{ get; private set; }
            /// <summary>
            /// Relative path to the file.
            /// </summary>
            public string FileName { get; private set; }
            /// <summary>
            /// Size of the file in bytes
            /// </summary>
            public long Size { get; private set; }
            /// <summary>
            /// Counts number of available parts
            /// </summary>
            public long Progress
            {
                get
                {
                    long ava = 0;
                    for (int i = 0; i < NumberOfParts; ++i)
                        if (PartStatus[i] == EPartStatus.Available)
                            ++ava;

                    return ava;
                }
            }



            /// <summary>
            /// Creates new file with specified path with info from xml
            /// </summary>
            /// <param name="elem">XmlElement to load from</param>
            /// <param name="filePath">Path to new file</param>
            /// <returns>The nrw PartFile</returns>
            public static PartFile FromXmlShare(XmlElement elem, string filePath)
            {
                PartFile file = new PartFile(filePath);

                file.NumberOfParts = int.Parse(elem["numberofparts"].InnerText);

                file.PartStatus = new EPartStatus[file.NumberOfParts];

                file.FileName = elem["filename"].InnerText;
                file.Hash = elem["hash"].InnerText;
                file.Size = long.Parse(elem["size"].InnerText);

                //file.setLength(file.Size);

                return file;
            }

            /// <summary>
            /// Creates new PartFile from existing file.
            /// </summary>
            /// <param name="filePath">Path of the file</param>
            /// <returns>Returns new PartFile</returns>
            public static PartFile FromPath(string filePath)
            {
                PartFile file = new PartFile(filePath);

                for (int i = 0; i < file.PartStatus.Length; ++i)
                    file.PartStatus[i] = EPartStatus.Available;

                return file;
            }

            /// <summary>
            /// Deserializes file from xml and opens it.
            /// </summary>
            /// <param name="elem">XmlElement with serializet filePart</param>
            /// <param name="checkHash"></param>
            /// <returns>The loaded PartFile</returns>
            public static PartFile FromXml(XmlElement elem, bool checkHash)
            {
                PartFile file = new PartFile(elem["filepath"].InnerText);

                /*if (checkHash && file.Hash != elem["hash"].InnerText)
                    throw new WrongFileException("File has changed");*/

                file.FileName = elem["filename"].InnerText;
                file.NumberOfParts = long.Parse(elem["numberofparts"].InnerText);
                file.PartStatus = new EPartStatus[file.NumberOfParts];
                string partstatus = elem["partstatus"].InnerText;

                for (int i = 0; i < partstatus.Length; ++i)
                {
                    file.PartStatus[i] = (EPartStatus)Enum.Parse(typeof(EPartStatus), partstatus[i].ToString());
                }



                return file;
            }

            /// <summary>
            /// Gets length of specified part in bytes
            /// </summary>
            /// <param name="part">0-based index of the part.</param>
            /// <returns>Returns length of the part in bytes</returns>
            public int GetPartLength(long part)
            {
                if (part == NumberOfParts - 1)
                    return (int)(Size % PartSize);
                return PartSize;
            }

            /// <summary>
            /// Serializes File into xml.
            /// </summary>
            /// <param name="doc">Context XmlDocument</param>
            /// <returns>XmlElement with serialised filePart</returns>
            public XmlElement SaveToXml(XmlDocument doc)
            {
                XmlElement elem = doc.CreateElement(XmlName);
                elem.AppendElementWithValue("filepath", FilePath);
                elem.AppendElementWithValue("numberofparts", NumberOfParts.ToString());
                elem.AppendElementWithValue("filename", FileName);
                elem.AppendElementWithValue("hash", Hash);
                elem.AppendElementWithValue("size", Size.ToString());


                StringBuilder sb = new StringBuilder();

                //Writes status of every part into the file as number (0 or 2). Processing parts are saved as missing.
                for (int i = 0; i < NumberOfParts; ++i)
                    sb.Append((int)(PartStatus[i] == EPartStatus.Processing ? EPartStatus.Missing : PartStatus[i]));

                elem.AppendElementWithValue("partstatus", sb.ToString());

                return elem;
            }

            /// <summary>
            /// Asynchrnonously reads spocified part into buffer
            /// </summary>
            /// <param name="buffer">Buffer into which the part will be read. In must be at least as big as the part.</param>
            /// <param name="part">0-based index of the part.</param>
            /// <returns></returns>
            public async Task ReadPartAsync(byte[] buffer, long part)
            {
                Seek(part);
                await stream.ReadAsync(buffer, 0, GetPartLength(part));

            }

            /// <summary>
            /// Closes the file.
            /// </summary>
            public void Close()
            {
                stream.Flush();
                stream.Close();
            }
            /// <summary>
            /// Asynchronously writes part into the file
            /// </summary>
            /// <param name="buffer">Bytes to write</param>
            /// <param name="part">0-based index of the part.</param>
            /// <returns></returns>
            public async Task WritePartAsync(byte[] buffer, long part)
            {
                await writeSemaphore.WaitAsync();

                try
                {
                    Seek(part);
                    await stream.WriteAsync(buffer, 0, GetPartLength(part));
                }
                catch (Exception)
                {
                    Logger.WriteLine("MISTAKE");
                }
                finally
                {
                    writeSemaphore.Release();
                }
            }

            /// <summary>
            /// Saves information about the file to write into share file.
            /// </summary>
            /// <param name="doc">Context XmlDocument</param>
            /// <returns>XmlElement with serialized PartFile</returns>
            public XmlElement SaveToXmlShare(XmlDocument doc)
            {
                XmlElement elem = doc.CreateElement(XmlName);
                elem.AppendElementWithValue("numberofparts", NumberOfParts.ToString());
                elem.AppendElementWithValue("filename", FileName);
                elem.AppendElementWithValue("hash", Hash);
                elem.AppendElementWithValue("size", Size.ToString());

                return elem;
            }

            public void Dispose()
            {
                writeSemaphore.Dispose();
                stream.Dispose();
            }

            /// <summary>
            /// Gets random index of part with specified status
            /// </summary>
            /// <param name="status"></param>
            /// <returns>returns 0-based index of the part, or -1 if no such part exists.</returns>
            public long GetPartIndex(EPartStatus status)
            {
                int count = 0;
                long i;

                //first it tries to choose randomly
                do
                {
                    ++count;
                    i = LongRandom(NumberOfParts);
                } while (PartStatus[i] != status && count < 30);

                if (PartStatus[i] == status)
                    return i;

                //after 30 tries, it just finds in ordered way.
                for (i = 0; i < NumberOfParts; ++i)
                    if (PartStatus[i] == status)
                        return i;

                return -1;
            }




            /// <summary>
            /// Sets position of head to specified part so it can be written or read.
            /// </summary>
            /// <param name="part">0-based index of the part.</param>
            private void Seek(long part)
            {
                stream.Seek(part * PartSize, SeekOrigin.Begin);
            }

            /// <summary>
            /// Sets length of the underlying file.
            /// </summary>
            /// <param name="length">The length in bytes</param>
            private void SetLength(long length)
            {
                stream.SetLength(length);
            }

            private long LongRandom(long max)
            {
                return LongRandom(0, max);
            }

            private long LongRandom(long min, long max)
            {
                byte[] buf = new byte[8];
                rnd.NextBytes(buf);
                long longRand = BitConverter.ToInt64(buf, 0);

                return (Math.Abs(longRand % (max - min)) + min);
            }

            /// <summary>
            /// Opens file and initializes separation into parts.
            /// </summary>
            /// <param name="filePath"></param>
            private void OpenFile(string filePath)
            {

                stream = File.Open(filePath, FileMode.OpenOrCreate);

                var split = filePath.Split('\\');
                FileName = split[split.Length - 1];
                Size = stream.Length;

                NumberOfParts = (Size - 1) / PartSize + 1;
                PartStatus = new EPartStatus[NumberOfParts];


                //Hash = hashFromStream(stream);
                Hash = "THISISJUSTLONGENOUGHSTRING0DEPRECATED";
            }

            private string HashFromStream(FileStream str)
            {
                var md5 = MD5.Create();

                return HashToString(md5.ComputeHash(str));
            }

            private static string HashToString(byte[] hash)
            {
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }
}
