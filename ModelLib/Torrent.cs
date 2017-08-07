using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EzShare
{
    namespace ModelLib
    {
        /// <summary>
        /// Represents one task to share file.
        /// </summary>
        public class Torrent : IDisposable
        {
            /// <summary>
            /// Creates fresh new torrent with randomly generated unique id, that does not change after.
            /// </summary>
            private Torrent()
            {
                downloadStopWatch.Start();
                uploadStopWatch.Start();
                Clients = new List<Client>();
                id = "";
                Random r = new Random();
                byte[] bytes = new byte[16];
                for (int i = 0; i < 16; ++i)
                    bytes[i] = (byte)r.Next(256);
                id = hashToString(bytes);
            }
            /// <summary>
            /// Creates new torrent with specified id.
            /// </summary>
            /// <param name="id">ID of torrent</param>
            private Torrent(string id)
            {
                Clients = new List<Client>();
                this.id = id;
            }

            /// <summary>
            /// Represents status of torrent
            /// </summary>
            public enum eStatus
            {
                Downloading,
                Paused,
                Seeding,
                Error
            }

            /// <summary>
            /// Adds new client that uses this torrent
            /// </summary>
            /// <param name="client">Client to add</param>
            public void AddClient(Client client)
            {
                client.ClientClosed += mc_ClientClosed;
                Clients.Add(client);
            }
            /// <summary>
            /// Removes client
            /// </summary>
            /// <param name="client">Client to remove</param>
            public void RemoveClient(Client client)
            {
                client.ClientClosed -= mc_ClientClosed;
                Clients.Remove(client);
            }
            /// <summary>
            /// When client was closed, it must be removed from Clients list.
            /// </summary>
            /// <param name="sender"></param>
            private void mc_ClientClosed(Client sender)
            {
                Clients.Remove(sender);
                sender.ClientClosed -= mc_ClientClosed;
            }

            /// <summary>
            /// Ip and port of all known devices with this torrent.
            /// </summary>
            public HashSet<ConnectInfo> ClientsInfo { get; set; } = new HashSet<ConnectInfo>();

            /// <summary>
            /// Represents status of this torrent.
            /// </summary>
            public virtual eStatus Status { get; set; }

            Stopwatch downloadStopWatch = new Stopwatch();
            Stopwatch uploadStopWatch = new Stopwatch();

            /// <summary>
            /// Actual speed of download
            /// </summary>
            public double DownloadSpeed
            {
                get
                {
                    long time = downloadStopWatch.ElapsedMilliseconds;
                    downloadStopWatch.Restart();
                    int downSum = 0;
                    for (int i = 0; i < Clients.Count; ++i)
                    {
                        downSum += Clients[i].DownloadedBytes;
                        Clients[i].DownloadedBytes = 0;
                    }

                    return downSum / (time / 1000d);
                }
            }
            /// <summary>
            /// Actual speed of upload.
            /// </summary>
            public double UploadSpeed
            {
                get
                {
                    long time = uploadStopWatch.ElapsedMilliseconds;
                    uploadStopWatch.Restart();
                    int upSum = 0;
                    for (int i = 0; i < Clients.Count; ++i)
                    {
                        upSum += Clients[i].UploadedBytes;
                        Clients[i].UploadedBytes = 0;
                    }

                    return upSum / (time / 1000d);
                }
            }

            private readonly string id;
            /// <summary>
            /// 32 chars long unique identification string of this torrent.
            /// </summary>
            public string Id
            {
                get { return id; }
            }

            
            /// <summary>
            /// Path to shared file.
            /// </summary>
            public virtual string FilePath
            {
                get { return File.FilePath; }
            }
            /// <summary>
            /// Relative path to shared file
            /// </summary>
            public virtual string FileName
            {
                get { return File.FileName; }
            }

            /// <summary>
            /// Returns number of available parts of shared file.
            /// </summary>
            public long ProgressOfFile
            {
                get { return File == null ? 0 : File.Progress; }
            }

            /// <summary>
            /// Clients that share or download this torrent.
            /// </summary>
            public List<Client> Clients
            {
                get;
                set;
            }
            /// <summary>
            /// Clients that download this torrent.
            /// </summary>
            public IEnumerable<DownClient> DownClients
            {
                get
                {
                    return Clients.OfType<DownClient>();
                }
            }
            /// <summary>
            /// Clients that upload this torrent.
            /// </summary>
            public IEnumerable<UpClient> UpClients
            {
                get
                {
                    return Clients.OfType<UpClient>();
                }
            }
            /// <summary>
            /// The file to share
            /// </summary>
            public PartFile File;
            /// <summary>
            /// Size of shared file in bytes
            /// </summary>
            public long Size
            {
                get { return File == null ? 0 : File.Size; }
            }
            /// <summary>
            /// Name of torrent (not unique).
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Number of parts file is divided into
            /// </summary>
            public long NumberOfParts { get { return File == null ? 0 : File.NumberOfParts; } }

            public static string XmlName = "torrent";
            /// <summary>
            /// Serializes torrent to xml
            /// </summary>
            /// <param name="doc"></param>
            /// <returns>XmlElement with serialized torrent</returns>
            public XmlElement SaveToXml(XmlDocument doc)
            {
                XmlElement e = doc.CreateElement(XmlName);

                e.AppendElementWithValue("name", Name);
                e.AppendElementWithValue("id", Id);
                e.AppendElementWithValue("status", Status.ToString());
                if (Status != eStatus.Error)
                    e.AppendChild(File.SaveToXml(doc));

                XmlElement clients = doc.CreateElement("clients");
                foreach (ConnectInfo info in this.ClientsInfo)
                    clients.AppendChild(info.SaveToXml(doc));
                e.AppendChild(clients);
                return e;
            }
            /// <summary>
            /// Serializes torrent in a way so it can be used in share file (omits information exclusive for this instance)
            /// </summary>
            /// <param name="doc">Conext XmlDocument</param>
            /// <returns>XmlElement with serialized torrent</returns>
            public XmlElement SaveToXmlShare(XmlDocument doc)
            {
                XmlElement e = doc.CreateElement(XmlName);

                e.AppendElementWithValue("name", Name);
                e.AppendElementWithValue("id", Id);
                e.AppendChild(File.SaveToXmlShare(doc));

                return e;
            }



            bool downloadIsRunning = false;
            /// <summary>
            /// Asynchronously downloads torrent using Clients.
            /// </summary>
            /// <returns></returns>
            public async Task Download()
            {
                downloadIsRunning = true;
                Status = Torrent.eStatus.Downloading;
                Logger.WriteLine("Starting download:" + Name);

                long part;
                var tasks = new Dictionary<Task<Client.eRequestPartResponse>, Tuple<DownClient, long>>();
                //creates tasks from all clients that are currently assigned to torrent - assignes different parts of file to different clients.
                foreach (var c in DownClients)
                {
                    part = File.GetPartIndex(PartFile.ePartStatus.Missing);
                    File.PartStatus[part] = PartFile.ePartStatus.Processing;
                    if (part == -1)
                        break;
                    tasks.Add(c.DownloadPart(File, part), Tuple.Create(c, part));
                }

                while (tasks.Count != 0)
                {//when one of the clients has finished downloading a part, another part is assigned.
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
                    //if there is still a missing part and torrent is still downloading(may be paused) and client is still functioning
                    if (part != -1 && Status == eStatus.Downloading && Clients.Contains(clientEnded))
                    {
                        File.PartStatus[part] = PartFile.ePartStatus.Processing;
                        tasks.Add(clientEnded.DownloadPart(File, part), Tuple.Create(clientEnded, part));
                    }

                }
                //if the file is downloaded
                if (Status == eStatus.Downloading && ProgressOfFile == NumberOfParts)
                {
                    Status = Torrent.eStatus.Seeding;
                    foreach (DownClient downClient in DownClients)
                        downClient.Close();
                }
                Logger.WriteLine("Ending download." + Name);
                downloadIsRunning = false;
            }

            /// <summary>
            /// Change status of torrent to paused
            /// </summary>
            public void Pause()
            {
                Status = eStatus.Paused;
            }

            /// <summary>
            /// Resumes torrent - either to Seeding state or resumes download.
            /// </summary>
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

            /// <summary>
            /// Releases unmanaged resources - closes clients and file.
            /// </summary>
            internal void Close()
            {
                File?.Close();
                foreach (Client client in Clients)
                    client.Close();
            }

            private static string hashToString(byte[] hash)
            {
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }

            /// <summary>
            /// Creates new torrent with specified file.
            /// </summary>
            /// <param name="path">Path to shared torrent</param>
            /// <returns>The newly created torrent</returns>
            public static Torrent CreateFromPath(string path)
            {
                Torrent t = new Torrent();
                t.File = PartFile.FromPath(path);

                t.Name = t.FileName;
                t.Status = eStatus.Seeding;
                return t;
            }

            /// <summary>
            /// Deserializes new torrent from xml - whole torrent is loaded
            /// </summary>
            /// <param name="elem">XmlElement with the torrent</param>
            /// <returns>The newly created torrent.</returns>
            public static Torrent CreateFromXml(XmlElement elem)
            {
                Torrent t = new Torrent(elem["id"].InnerText);
                t.Name = elem["name"].InnerText;
                t.Status = (eStatus)Enum.Parse(typeof(eStatus), elem["status"].InnerText);
                if (t.Status != eStatus.Error)
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
            /// <summary>
            /// Creates new torrent from xml from share file
            /// </summary>
            /// <param name="elem">Xml element with the torrent</param>
            /// <param name="filePath">Path where this torrent will be downloaded on this device</param>
            /// <returns>The newly created torrent.</returns>
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
                foreach (Client client in Clients)
                    client.Dispose();
            }
        }
    }
}


