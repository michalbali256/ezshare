using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Xml;

using EzShare.ModelLib;
using System.IO;

namespace EzShare
{
    namespace GUI
    {
        /// <summary>
        /// The main window of application
        /// </summary>
        public partial class Main : Form, IDisposable
        {
            public Main()
            {
                InitializeComponent();
                Logger.WroteLine += Logger_WriteLineE;
                Logger.WriteLine("Loading settings.xml");
            }

            /// <summary>
            /// Everything is logged into listBox.
            /// </summary>
            /// <param name="line"></param>
            private void Logger_WriteLineE(string line)
            {
                listBoxLog.Items.Add(line);
            }


            TorrentManager manager;

            /// <summary>
            /// Updates every row of table based objects of type Torrent in row.Tag
            /// </summary>
            private void updateTable()
            {
                foreach (DataGridViewRow r in dataGridView.Rows)
                    updateRow(r);
            }

            /// <summary>
            /// Adds torrent to table
            /// </summary>
            /// <param name="torrent"></param>
            private void addRow(Torrent torrent)
            {
                DataGridViewRow r = new DataGridViewRow();
                r.Tag = torrent;
                r.ContextMenuStrip = contextMenuStripRow;
                r.CreateCells(dataGridView);
                updateRow(r);
                dataGridView.Rows.Add(r);
            }

            /// <summary>
            /// Updates row according to Torrent saved in Tag property
            /// </summary>
            /// <param name="dataGridViewRow">The row to update</param>
            private void updateRow(DataGridViewRow dataGridViewRow)
            {
                Torrent t = dataGridViewRow.Tag as Torrent;
                if (t == null)
                    return;
                dataGridViewRow.Cells[0].Value = t.Name;
                dataGridViewRow.Cells[1].Value = t.ProgressOfFile.ToString() + "/" + t.NumberOfParts; ;
                dataGridViewRow.Cells[2].Value = t.Status.ToString();
                dataGridViewRow.Cells[3].Value = normalizeSize(t.Size);
                dataGridViewRow.Cells[4].Value = normalizeSpeed(t.DownloadSpeed);
                dataGridViewRow.Cells[5].Value = normalizeSpeed(t.UploadSpeed);
                dataGridViewRow.Cells[6].Value = t.Clients.Count;
            }

            string[] units = {"B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB" };

            /// <summary>
            /// Adds proper unit of measurement to the size.
            /// </summary>
            /// <param name="size">The size in bytes</param>
            /// <returns>Returns the size in such units that number is < 1024</returns>
            private string normalizeSize(long size)
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
            /// <returns>Returns the speed in such units that number is < 1024</returns>
            private string normalizeSpeed(double speed)
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
            /// Adds new torrent based on openFileDialog choice of user
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void toolStripButtonAdd_Click(object sender, EventArgs e)
            {
                if (openFileDialogFile.ShowDialog() != DialogResult.OK)
                    return;
                Torrent t = Torrent.CreateFromPath(openFileDialogFile.FileName);
                addRow(t);
                manager.Add(t);
            }

            const string settingsFile = "settings.xml";
            const string xmlName = "settings";

            /// <summary>
            /// Saves settings when form is closed
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void Main_FormClosed(object sender, FormClosedEventArgs e)
            {
                XmlDocument doc = new XmlDocument();
                XmlElement settings = doc.CreateElement(xmlName);
                settings.AppendChild(manager.SaveToXml(doc));
                doc.AppendChild(settings);
                doc.Save(settingsFile);
                Logger.Close();
            }

            /// <summary>
            /// Loads settings from file (or sets default ones if not available) and starts manager
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private async void Main_Load(object sender, EventArgs e)
            {
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(settingsFile);
                    manager = TorrentManager.FromXml(doc[xmlName][TorrentManager.XmlName]);
                    Logger.WriteLine("Loaded settings.xml");
                }
                catch (System.IO.IOException)
                {
                    Logger.WriteLine("Loading settings.xml failed, using default settings");
                    manager = new TorrentManager();

                }

                dataGridView.Rows.Clear();
                foreach (var t in manager)
                {
                    addRow(t);
                }

                var task = manager.StartListeningAsync();
                
                timerUpdate.Enabled = true;

                await manager.ConnectAllDownloadingTorrentsAsync();
            }

            /// <summary>
            /// Opens properties window
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Torrent t = (Torrent)dataGridView.SelectedRows[0].Tag;
                TorrentProperties f = new TorrentProperties(t, false);
                if (f.ShowDialog() != DialogResult.OK)
                    return;
            }

            /// <summary>
            /// Row is selected on right click, for context menu to determine which row is selected
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void dataGridView_MouseDown(object sender, MouseEventArgs e)
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
            /// Saves share file of selected torrent in table
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void saveshareFileToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Torrent torrent = (Torrent)dataGridView.SelectedRows[0].Tag;

                saveShareFile(torrent);
            }

            /// <summary>
            /// Opens save dialog and saves .share file to user-specified location
            /// </summary>
            /// <param name="torrent">The torrent of which to save .share file</param>
            private void saveShareFile(Torrent torrent)
            {
                if (saveFileDialogShare.ShowDialog() != DialogResult.OK)
                    return;
                try
                {
                    saveShareFile(torrent, saveFileDialogShare.FileName);
                }
                catch (IOException exception)
                {
                    MessageBox.Show("Could not save to specified location: " + saveFileDialogFile.FileName + " Reason: " + exception.Message);
                }
            }

            /// <summary>
            /// Saves share file of specified torrent to specified location.
            /// </summary>
            /// <param name="torrent">The torrent of which to save .share file</param>
            /// <param name="shareFileName">File name of new .share file</param>
            /// <exception cref="IOException"></exception>
            private void saveShareFile(Torrent torrent, string shareFileName)
            {

                XmlDocument doc = new XmlDocument();
                XmlElement documentElement = doc.CreateElement("share");
                documentElement.AppendChild(manager.MyConnectInfo.SaveToXml(doc));
                documentElement.AppendChild(torrent.SaveToXmlShare(doc));

                doc.AppendChild(documentElement);
                try
                {
                    doc.Save(shareFileName);
                    Logger.WriteLine("Successfuly written " + shareFileName);
                }
                catch (IOException exception)
                {
                    Logger.WriteLine("Could not save to specified location: " + shareFileName + " Reason: " + exception.Message);
                    throw;
                }

            }


            /// <summary>
            /// Starts all selected torrents
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void toolStripButtonStart_Click(object sender, EventArgs e)
            {
                foreach (DataGridViewRow r in dataGridView.SelectedRows)
                {
                    Torrent t = r.Tag as Torrent;
                    t.Start();
                }
            }

            /// <summary>
            /// Allows user to select share file and select location to download torrent and starts download
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private async void toolStripButtonConnect_Click(object sender, EventArgs e)
            {
                //open file dialog to select 
                if (openFileDialogTorrent.ShowDialog() != DialogResult.OK)
                    return;
                XmlDocument document = new XmlDocument();
                Torrent torrent;
                ConnectInfo connectInfo = new ConnectInfo();
                if (saveFileDialogFile.ShowDialog() != DialogResult.OK)
                    return;
                try
                {
                    //loads selected share file end parses torrent and information about remote host
                    document.Load(openFileDialogTorrent.FileName);
                    XmlElement headerElement = document["share"][ConnectInfo.XmlName];
                    XmlElement torrentElement = document["share"][Torrent.XmlName];

                    torrent = Torrent.CreateFromXmlShare(torrentElement, saveFileDialogFile.FileName);
                    connectInfo = ConnectInfo.ParseXml(headerElement);


                    addRow(torrent);
                    manager.Add(torrent);
                    try
                    {
                        await manager.ConnectTorrentAsync(torrent, connectInfo);
                        await torrent.Download();
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
                catch (System.IO.IOException ex)
                {
                    Logger.WriteLine("Couldn't open file. " + ex.ToString());
                    MessageBox.Show("Couldn't open file. " + ex.ToString());

                }
                catch (XmlException ex)
                {
                    Logger.WriteLine("File has wrong format. " + ex.ToString());
                    MessageBox.Show("File has wrong format. " + ex.ToString());
                }

            }

            /// <summary>
            /// Removes selected torrents from table and manager
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void toolStripButtonRemove_Click(object sender, EventArgs e)
            {
                foreach (DataGridViewRow r in dataGridView.SelectedRows)
                {
                    manager.Remove((Torrent)r.Tag);
                    dataGridView.Rows.Remove(r);
                }
            }

            /// <summary>
            /// Updates table.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void timerUpdate_Tick(object sender, EventArgs e)
            {
                updateTable();
            }

            /// <summary>
            /// Pauses selected torrents
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void toolStripButtonPause_Click(object sender, EventArgs e)
            {
                foreach (DataGridViewRow r in dataGridView.SelectedRows)
                {
                    Torrent t = r.Tag as Torrent;
                    t?.Pause();
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

            /// <summary>
            /// Deletes all torrents.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void clearAllTorrentsToolStripMenuItem_Click(object sender, EventArgs e)
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
            private void startAllToolStripMenuItem_Click(object sender, EventArgs e)
            {
                foreach (DataGridViewRow r in dataGridView.SelectedRows)
                {
                    Torrent t = r.Tag as Torrent;
                    t?.Start();
                }
            }

            /// <summary>
            /// Closes window and application.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void exitToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Close();
            }

            /// <summary>
            /// Shows about window
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
            {
                MessageBox.Show("ezShare\n  v1.0\n Michal Bali");
            }

            private const string shareExtension = ".share";

            /// <summary>
            /// Lets user choose a folder and saves .share files of all torrents to the folder.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void saveshareFilesOfAllTorrentsToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                    return;

                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    Torrent torrent = row.Tag as Torrent;
                    if (torrent != null)
                    {
                        string fileName = folderBrowserDialog1.SelectedPath + "\\" + torrent.Name + shareExtension;
                        try
                        {
                            saveShareFile(torrent, fileName);
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
            private void reconnectAllClientsToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Torrent torrent = (Torrent)dataGridView.SelectedRows[0].Tag;
                manager.ConnectDownloadingTorrentAsync(torrent);
            }

            /// <summary>
            /// Starts selected torrent.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void startToolStripMenuItem_Click(object sender, EventArgs e)
            {
                //this should be executed after right click was pressed on a row - only one row is selected
                Torrent torrent = (Torrent)dataGridView.SelectedRows[0].Tag;
                torrent?.Start();
            }

            /// <summary>
            /// Pauses selected torrent
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
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
            private void contextMenuStripRow_Opening(object sender, System.ComponentModel.CancelEventArgs e)
            {
                Torrent torrent = (Torrent)dataGridView.SelectedRows[0].Tag;
                if (torrent.Status != Torrent.eStatus.Downloading)
                    reconnectDownloadingClientsToolStripMenuItem.Enabled = false;
            }
        }
    }
}