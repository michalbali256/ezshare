using System;
using System.Collections.Generic;
using System.Linq;
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
            public const string XmlName = "torrent";
            /// <summary>
            /// Ip and port of all known devices with this torrent.
            /// </summary>
            public HashSet<ConnectInfo> ClientsInfo { get; set; } = new HashSet<ConnectInfo>();

            Stopwatch downloadStopWatch = new Stopwatch();
            Stopwatch uploadStopWatch = new Stopwatch();

            private bool downloadIsRunning = false;



            /// <summary>
            /// Creates fresh new torrent with randomly generated unique id, that does not change after.
            /// </summary>
            private Torrent()
            {
                downloadStopWatch.Start();
                uploadStopWatch.Start();
                Clients = new List<Client>();
                Id = "";
                Random r = new Random();
                byte[] bytes = new byte[16];
                for (int i = 0; i < 16; ++i)
                    bytes[i] = (byte)r.Next(256);
                Id = HashToString(bytes);
            }
            /// <summary>
            /// Creates new torrent with specified id.
            /// </summary>
            /// <param name="id">ID of torrent</param>
            private Torrent(string id)
            {
                Clients = new List<Client>();
                Id = id;
            }



            /// <summary>
            /// Represents status of torrent
            /// </summary>
            public enum EStatus
            {
                Downloading,
                Paused,
                Seeding,
                Error
            }



            /// <summary>
            /// 32 chars long unique identification string of this torrent.
            /// </summary>
            public string Id { get; }

            /// <summary>
            /// Represents status of this torrent.
            /// </summary>
            public virtual EStatus Status { get; protected set; }

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
            /// <summary>
            /// The file to share
            /// </summary>
            public PartFile File { get; private set; }

            /// <summary>
            /// Path to shared file.
            /// </summary>
            public virtual string FilePath => File.FilePath;

            /// <summary>
            /// Relative path to shared file
            /// </summary>
            public virtual string FileName => File.FileName;

            /// <summary>
            /// Returns number of available parts of shared file.
            /// </summary>
            public long ProgressOfFile => File?.Progress ?? 0;

            /// <summary>
            /// Clients that share or download this torrent.
            /// </summary>
            public List<Client> Clients
            {
                get;
            }
            /// <summary>
            /// Clients that download this torrent.
            /// </summary>
            public IEnumerable<DownClient> DownClients => Clients.OfType<DownClient>();

            /// <summary>
            /// Clients that upload this torrent.
            /// </summary>
            public IEnumerable<UpClient> UpClients => Clients.OfType<UpClient>();

            /// <summary>
            /// Size of shared file in bytes
            /// </summary>
            public long Size => File?.Size ?? 0;

            /// <summary>
            /// Name of torrent (not unique).
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Number of parts file is divided into
            /// </summary>
            public long NumberOfParts => File?.NumberOfParts ?? 0;



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
                t.Status = EStatus.Seeding;
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
                t.Status = (EStatus)Enum.Parse(typeof(EStatus), elem["status"].InnerText);
                if (t.Status != EStatus.Error)
                    try
                    {
                        t.File = PartFile.FromXml(elem[PartFile.XmlName], t.Status == EStatus.Seeding); //check hash only if the the file is already downloaded - otherwise the hash must be different
                    }
                    catch (WrongFileException)
                    {
                        t.Status = EStatus.Error;
                    }
                    catch (IOException)
                    {
                        t.Status = EStatus.Error;
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
                if (Status != EStatus.Error)
                    e.AppendChild(File.SaveToXml(doc));

                XmlElement clients = doc.CreateElement("clients");
                foreach (ConnectInfo info in ClientsInfo)
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

            /// <summary>
            /// Adds new client that uses this torrent
            /// </summary>
            /// <param name="client">Client to add</param>
            public void AddClient(Client client)
            {
                client.ClientClosed += Mc_ClientClosed;
                Clients.Add(client);
            }
            /// <summary>
            /// Removes client
            /// </summary>
            /// <param name="client">Client to remove</param>
            public void RemoveClient(Client client)
            {
                client.ClientClosed -= Mc_ClientClosed;
                Clients.Remove(client);
            }
            
            /// <summary>
            /// Asynchronously downloads torrent using Clients.
            /// </summary>
            /// <returns></returns>
            public async Task DownloadAsync()
            {
                downloadIsRunning = true;
                Status = EStatus.Downloading;
                Logger.WriteLine("Starting download:" + Name);

                long part;
                var tasks = new Dictionary<Task<Client.ERequestPartResponse>, Tuple<DownClient, long>>();
                //creates tasks from all clients that are currently assigned to torrent - assignes different parts of file to different clients.
                foreach (var c in DownClients)
                {
                    part = File.GetPartIndex(PartFile.EPartStatus.Missing);
                    File.PartStatus[part] = PartFile.EPartStatus.Processing;
                    if (part == -1)
                        break;
                    tasks.Add(c.DownloadPartAsync(File, part), Tuple.Create(c, part));
                }

                while (tasks.Count != 0)
                {//when one of the clients has finished downloading a part, another part is assigned.
                    var t = await Task.WhenAny(tasks.Keys);

                    DownClient clientEnded = tasks[t].Item1;
                    long partEnded = tasks[t].Item2;

                    if (t.Result == Client.ERequestPartResponse.OK)
                    {
                        File.PartStatus[partEnded] = PartFile.EPartStatus.Available;
                        Logger.WriteLine("Part transfered successfuly.");
                    }
                    else
                        File.PartStatus[partEnded] = PartFile.EPartStatus.Missing;
                    tasks.Remove(t);

                    part = File.GetPartIndex(PartFile.EPartStatus.Missing);
                    //if there is still a missing part and torrent is still downloading(may be paused) and client is still functioning
                    if (part != -1 && Status == EStatus.Downloading && Clients.Contains(clientEnded))
                    {
                        File.PartStatus[part] = PartFile.EPartStatus.Processing;
                        tasks.Add(clientEnded.DownloadPartAsync(File, part), Tuple.Create(clientEnded, part));
                    }

                }
                //if the file is downloaded
                if (Status == EStatus.Downloading && ProgressOfFile == NumberOfParts)
                {
                    Status = EStatus.Seeding;
                    foreach (DownClient downClient in DownClients.ToList())
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
                if(Status != EStatus.Error)
                    Status = EStatus.Paused;
            }

            /// <summary>
            /// Resumes torrent - either to Seeding state or resumes download.
            /// </summary>
            public async void StartAsync()
            {
                if (Status == EStatus.Error)
                {
                    Logger.WriteLine("Cannot start torrent with error");
                    return;
                }

                if (ProgressOfFile == NumberOfParts)
                {
                    Status = EStatus.Seeding;
                }
                else
                {
                    Status = EStatus.Downloading;
                    if (!downloadIsRunning)
                        await DownloadAsync();
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

            public void Dispose()
            {
                File.Dispose();
                foreach (Client client in Clients)
                    client.Dispose();
            }

            public void DisconnectAllClients()
            {
                foreach (DownClient downClient in DownClients.ToList())
                    downClient.Close();
            }

            private static string HashToString(byte[] hash)
            {
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
            /// <summary>
            /// When client was closed, it must be removed from Clients list.
            /// </summary>
            /// <param name="sender"></param>
            private void Mc_ClientClosed(Client sender)
            {
                Clients.Remove(sender);
                sender.ClientClosed -= Mc_ClientClosed;
            }
        }
    }
}


