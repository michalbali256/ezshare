using System;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using EzShare.ModelLib;

namespace EzShare
{
    namespace GUI
    {
        /// <summary>
        /// The main window of application
        /// </summary>
        public partial class Main : Form
        {
            private const string SettingsFile = "settings.xml";
            private const string XmlName = "settings";
            private const string ShareExtension = ".share";

            private TorrentManager manager;
            private readonly string[] units = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB" };



            public Main()
            {
                InitializeComponent();
                Logger.WroteLine += Logger_WriteLineE;
            }


            /// <summary>
            /// Everything is logged into listBox.
            /// </summary>
            /// <param name="line"></param>
            private void Logger_WriteLineE(string line)
            {
                listBoxLog.Items.Add(line);
            }

            /// <summary>
            /// Saves settings when form is closed
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void Main_FormClosed(object sender, FormClosedEventArgs e)
            {
                XmlDocument doc = new XmlDocument();
                XmlElement settings = doc.CreateElement(XmlName);
                settings.AppendChild(manager.SaveToXml(doc));
                doc.AppendChild(settings);
                doc.Save(SettingsFile);
                Logger.Close();
            }

            /// <summary>
            /// Loads settings from file (or sets default ones if not available) and starts manager
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private async void Main_Load(object sender, EventArgs e)
            {
                Logger.Initialise(AppDomain.CurrentDomain.FriendlyName + ".log");

                Logger.WriteLine("Loading settings.xml");
                XmlDocument doc = new XmlDocument();

                try
                {
                    try
                    {
                        doc.Load(SettingsFile);
                        manager = TorrentManager.FromXml(doc[XmlName][TorrentManager.XmlName]);
                        Logger.WriteLine("Loaded settings.xml");
                    }
                    catch (IOException)
                    {
                        Logger.WriteLine("Loading settings.xml failed, using default settings");
                        manager = new TorrentManager();
                    }
                    catch (NullReferenceException)
                    {
                        Logger.WriteLine("Loading settings.xml failed, using default settings");
                        manager = new TorrentManager();
                    }
                }
                catch (SocketException exception)
                {
                    Logger.WriteLine(exception.Message);
                    Logger.WriteLine("Unable to determine IP automatically. Please edit settings.xml.");
                    MessageBox.Show("Unable to determine IP automatically. Please edit settings.xml.");
                    manager = new TorrentManager(new byte[] { 127, 0, 0, 1 });
                    Close();
                }

                dataGridView.Rows.Clear();
                foreach (var t in manager)
                {
                    AddRow(t);
                }


                if (TorrentManager.IsPortUsed(manager.MyConnectInfo))
                {
                    Logger.WriteLine("The port " + manager.MyConnectInfo.Port + " is already in use. Please change settings.xml to use another port.");
                    MessageBox.Show("The port " + manager.MyConnectInfo.Port + " is already in use. Please change settings.xml to use another port.");
                    Close();
                }

                manager.StartListeningAsync();

                timerUpdate.Enabled = true;

                await manager.ConnectAllDownloadingTorrentsAsync();
            }



            #region Table

            /// <summary>
            /// Updates table.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void timerUpdate_Tick(object sender, EventArgs e)
            {
                UpdateTable();
            }

            /// <summary>
            /// Updates every row of table based objects of type Torrent in row.Tag
            /// </summary>
            private void UpdateTable()
            {
                foreach (DataGridViewRow r in dataGridView.Rows)
                    UpdateRow(r);
            }

            /// <summary>
            /// Adds torrent to table
            /// </summary>
            /// <param name="torrent"></param>
            private void AddRow(Torrent torrent)
            {
                var r = new DataGridViewRow();
                r.Tag = torrent;
                r.ContextMenuStrip = contextMenuStripRow;
                r.CreateCells(dataGridView);
                UpdateRow(r);
                dataGridView.Rows.Add(r);
            }

            /// <summary>
            /// Updates row according to Torrent saved in Tag property
            /// </summary>
            /// <param name="dataGridViewRow">The row to update</param>
            private void UpdateRow(DataGridViewRow dataGridViewRow)
            {
                if (!(dataGridViewRow.Tag is Torrent t))
                    return;
                dataGridViewRow.Cells[0].Value = t.Name;
                dataGridViewRow.Cells[1].Value = t.ProgressOfFile + "/" + t.NumberOfParts;
                dataGridViewRow.Cells[2].Value = t.Status.ToString();
                dataGridViewRow.Cells[3].Value = NormalizeSize(t.Size);
                dataGridViewRow.Cells[4].Value = NormalizeSpeed(t.DownloadSpeed);
                dataGridViewRow.Cells[5].Value = NormalizeSpeed(t.UploadSpeed);
                dataGridViewRow.Cells[6].Value = t.Clients.Count;
            }

            /// <summary>
            /// Adds proper unit of measurement to the size.
            /// </summary>
            /// <param name="size">The size in bytes</param>
            /// <returns>Returns the size in such units that number is &lt; 1024</returns>
            private string NormalizeSize(long size)
            {
                double siz = size;
                int unit = 0;
                while (siz > 1024)
                {
                    ++unit;
                    siz = siz / 1024;
                }

                return Math.Round(siz, 1) + units[unit];
            }

            /// <summary>
            /// Adds proper unit of measurement to the size.
            /// </summary>
            /// <param name="speed">The size in bytes</param>
            /// <returns>Returns the speed in such units that number is &lt; 1024</returns>
            private string NormalizeSpeed(double speed)
            {
                int unit = 0;
                while (speed > 1024)
                {
                    ++unit;
                    speed = speed / 1024;
                }

                if (double.IsNaN(speed))
                    speed = 0;

                return Math.Round(speed, 1) + units[unit] + "/s";
            }

            /// <summary>
            /// Row is selected on right click, for context menu to determine which row is selected
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void DataGridView_MouseDown(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right)
                {
                    var hti = dataGridView.HitTest(e.X, e.Y);
                    dataGridView.ClearSelection();
                    //Only if the row exists
                    if (hti.RowIndex >= 0 && hti.RowIndex < dataGridView.Rows.Count)
                        dataGridView.Rows[hti.RowIndex].Selected = true;
                }
            }

            /// <summary>
            /// Deletes all selected torrents
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void dataGridView_KeyDown(object sender, KeyEventArgs e)
            {
                switch (e.KeyCode)
                {
                    case Keys.Delete:
                        foreach (DataGridViewRow r in dataGridView.SelectedRows)
                        {
                            manager.Remove((Torrent)r.Tag);
                            dataGridView.Rows.Remove(r);
                        }
                        break;
                }
            }
            #endregion



            /// <summary>
            /// Adds new torrent based on openFileDialog choice of user
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void ToolStripButtonAdd_Click(object sender, EventArgs e)
            {
                if (openFileDialogFile.ShowDialog() != DialogResult.OK)
                    return;
                Torrent t = Torrent.CreateFromPath(openFileDialogFile.FileName);
                AddRow(t);
                manager.Add(t);
            }

            /// <summary>
            /// Opens properties window
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void PropertiesToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Torrent t = (Torrent)dataGridView.SelectedRows[0].Tag;
                TorrentProperties f = new TorrentProperties(t, false);
                f.ShowDialog();
            }

            /// <summary>
            /// Saves share file of selected torrent in table
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void SaveshareFileToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Torrent torrent = (Torrent)dataGridView.SelectedRows[0].Tag;

                SaveShareFile(torrent);
            }

            /// <summary>
            /// Opens save dialog and saves .share file to user-specified location
            /// </summary>
            /// <param name="torrent">The torrent of which to save .share file</param>
            private void SaveShareFile(Torrent torrent)
            {
                if (saveFileDialogShare.ShowDialog() != DialogResult.OK)
                    return;
                try
                {
                    manager.SaveShareFile(torrent, saveFileDialogShare.FileName);
                }
                catch (IOException exception)
                {
                    MessageBox.Show("Could not save to specified location: " + saveFileDialogFile.FileName + " Reason: " + exception.Message);
                }
            }

            /// <summary>
            /// Starts all selected torrents
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void ToolStripButtonStart_Click(object sender, EventArgs e)
            {
                foreach (DataGridViewRow r in dataGridView.SelectedRows)
                {
                    if (!(r.Tag is Torrent t))
                        continue;
                    t.StartAsync();
                }
            }

            /// <summary>
            /// Allows user to select share file and select location to download torrent and starts download
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private async void ToolStripButtonConnect_Click(object sender, EventArgs e)
            {
                //open file dialog to select 
                if (openFileDialogTorrent.ShowDialog() != DialogResult.OK)
                    return;
                XmlDocument document = new XmlDocument();
                Torrent torrent;
                
                if (saveFileDialogFile.ShowDialog() != DialogResult.OK)
                    return;
                try
                {
                    //loads selected share file end parses torrent and information about remote host
                    document.Load(openFileDialogTorrent.FileName);
                    XmlElement headerElement = document["share"][ConnectInfo.XmlName];
                    XmlElement torrentElement = document["share"][Torrent.XmlName];

                    torrent = Torrent.CreateFromXmlShare(torrentElement, saveFileDialogFile.FileName);
                    ConnectInfo connectInfo = ConnectInfo.ParseXml(headerElement);


                    AddRow(torrent);
                    manager.Add(torrent);
                    try
                    {
                        await manager.ConnectTorrentAsync(torrent, connectInfo);
                        await torrent.DownloadAsync();
                    }
                    catch (SocketException)
                    {
                        Logger.WriteLine("Unable to connect to specified host.");
                    }
                    catch (AggregateException ex)
                    {
                        Logger.WriteLine("Aggregate exception caught.");
                        Logger.WriteLine(ex.InnerExceptions[0].Message);

                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(ex.Message);
                    }

                }
                catch (IOException ex)
                {
                    Logger.WriteLine("Couldn't open file. " + ex);
                    MessageBox.Show("Couldn't open file. " + ex);

                }
                catch (XmlException ex)
                {
                    Logger.WriteLine("File has wrong format. " + ex);
                    MessageBox.Show("File has wrong format. " + ex);
                }
                catch (NullReferenceException ex)
                {
                    Logger.WriteLine("File has wrong format. " + ex);
                    MessageBox.Show("File has wrong format. " + ex);
                }

            }

            /// <summary>
            /// Removes selected torrents from table and manager
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void ToolStripButtonRemove_Click(object sender, EventArgs e)
            {
                foreach (DataGridViewRow r in dataGridView.SelectedRows)
                {
                    manager.Remove((Torrent)r.Tag);
                    dataGridView.Rows.Remove(r);
                }
            }

            /// <summary>
            /// Pauses selected torrents
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void ToolStripButtonPause_Click(object sender, EventArgs e)
            {
                foreach (DataGridViewRow r in dataGridView.SelectedRows)
                {
                    Torrent t = r.Tag as Torrent;
                    t?.Pause();
                }
            }

            /// <summary>
            /// Deletes all torrents.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void ClearAllTorrentsToolStripMenuItem_Click(object sender, EventArgs e)
            {
                while(dataGridView.RowCount > 0)
                {
                    DataGridViewRow r = dataGridView.Rows[0];
                    manager.Remove((Torrent)r.Tag);
                    dataGridView.Rows.Remove(r);
                }
            }

            /// <summary>
            /// Starts all torrents.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void StartAllToolStripMenuItem_Click(object sender, EventArgs e)
            {
                foreach (DataGridViewRow r in dataGridView.SelectedRows)
                {
                    Torrent t = r.Tag as Torrent;
                    t?.StartAsync();
                }
            }

            /// <summary>
            /// Closes window and application.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Close();
            }

            /// <summary>
            /// Shows about window
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
            {
                MessageBox.Show("ezShare\n  v1.0\n Michal Bali");
            }

            /// <summary>
            /// Lets user choose a folder and saves .share files of all torrents to the folder.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void SaveshareFilesOfAllTorrentsToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                    return;

                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    Torrent torrent = row.Tag as Torrent;
                    if (torrent != null)
                    {
                        string fileName = folderBrowserDialog1.SelectedPath + "\\" + torrent.Name + ShareExtension;
                        try
                        {
                            manager.SaveShareFile(torrent, fileName);
                        }
                        catch (IOException exception)
                        {
                            MessageBox.Show("Could not save to specified location: " + fileName + " Reason: " + exception.Message);
                        }
                    }
                }
                
            }

            /// <summary>
            /// Connects all clients in connectInfo of the selected torrent.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void ReconnectAllClientsToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Torrent torrent = (Torrent)dataGridView.SelectedRows[0].Tag;
                manager.ConnectDownloadingTorrentAsync(torrent);
            }

            /// <summary>
            /// Starts selected torrent.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void StartToolStripMenuItem_Click(object sender, EventArgs e)
            {
                //this should be executed after right click was pressed on a row - only one row is selected
                Torrent torrent = (Torrent)dataGridView.SelectedRows[0].Tag;
                torrent?.StartAsync();
            }

            /// <summary>
            /// Pauses selected torrent
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void PauseToolStripMenuItem_Click(object sender, EventArgs e)
            {
                //this should be executed after right click was pressed on a row - only one row is selected
                Torrent torrent = (Torrent)dataGridView.SelectedRows[0].Tag;
                torrent?.Pause();
            }

            /// <summary>
            /// If status of torrent is not downloading, it makes no sense to connect donwloading clients.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void ContextMenuStripRow_Opening(object sender, System.ComponentModel.CancelEventArgs e)
            {
                Torrent torrent = (Torrent)dataGridView.SelectedRows[0].Tag;
                if (torrent.Status != Torrent.EStatus.Downloading)
                    reconnectDownloadingClientsToolStripMenuItem.Enabled = false;
            }
        }
    }
}