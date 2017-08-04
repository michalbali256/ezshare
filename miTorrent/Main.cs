using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Xml;

namespace miTorrent
{
    public partial class Main : Form, IDisposable
    {
        public Main()
        {
            InitializeComponent();
            Logger.OnWriteLine += Logger_WriteLineE;
            Logger.WriteLine("Loading settings.xml");
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(settingsFile);
                manager = TorrentManager.FromXml(doc[xmlName][TorrentManager.XmlName]);
                Logger.WriteLine("Loaded settings.xml");
            }
            catch (System.IO.IOException ex)
            {
                Logger.WriteLine("Loading settings.xml failed, using default settings");
                manager = new TorrentManager();
            }

            dataGridView.Rows.Clear();
            foreach (var t in manager)
            {
                addRow(t);
            }

            manager.StartListening();
        }

        private void Logger_WriteLineE(string obj)
        {
            listBoxLog.Items.Add(obj);
        }

        TorrentManager manager;

        private void updateTable()
        {
            foreach (DataGridViewRow r in dataGridView.Rows)
                updateRow(r);
        }

        private void addRow(Torrent t)
        {
            DataGridViewRow r = new DataGridViewRow();
            r.Tag = t;
            r.ContextMenuStrip = contextMenuStripRow;
            r.CreateCells(dataGridView);
            updateRow(r);
            dataGridView.Rows.Add(r);
        }

        private void updateRow(DataGridViewRow r)
        {
            Torrent t = r.Tag as Torrent;
            if (t == null)
                return;
            r.Cells[0].Value = t.Name;
            r.Cells[1].Value = t.ProgressOfFile.ToString() + "/" + t.NumberOfParts; ;
            r.Cells[2].Value = t.Status.ToString();
            r.Cells[3].Value = normalizeSize(t.Size);
        }

        private string normalizeSize(long size)
        {
            return size.ToString();
        }

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
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement settings = doc.CreateElement(xmlName);
            settings.AppendChild(manager.SaveToXml(doc));
            doc.AppendChild(settings);
            doc.Save(settingsFile);
            Logger.sw.Close();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            timerUpdate.Enabled = true;
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Torrent t = (Torrent)dataGridView.SelectedRows[0].Tag;
            TorrentProperties f = new TorrentProperties(t, false);
            if (f.ShowDialog() != DialogResult.OK)
                return;
        }

        private void contextMenuStripRow_Opened(object sender, EventArgs e)
        {

        }

        private void dataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hti = dataGridView.HitTest(e.X, e.Y);
                dataGridView.ClearSelection();
                if(hti.RowIndex >= 0 && hti.RowIndex < dataGridView.Rows.Count)
                    dataGridView.Rows[hti.RowIndex].Selected = true;
            }
        }

        private void saveshareFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialogShare.ShowDialog() != DialogResult.OK)
                return;

            Torrent t = (Torrent)dataGridView.SelectedRows[0].Tag;

            XmlDocument doc = new XmlDocument();
            XmlElement documentElement = doc.CreateElement("share");
            documentElement.AppendChild(manager.MyConnectInfo.ShareHeaderToXml(doc));
            documentElement.AppendChild(t.SaveToXmlShare(doc));

            doc.AppendChild(documentElement);
            doc.Save(saveFileDialogShare.FileName);
        }

        private void toolStripButtonStart_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in dataGridView.SelectedRows)
            {
                Torrent t = r.Tag as Torrent;
                t.Start();
            }
        }

        private async void toolStripButtonConnect_Click(object sender, EventArgs e)
        {
            if (openFileDialogTorrent.ShowDialog() != DialogResult.OK)
                return;
            XmlDocument doc = new XmlDocument();
            Torrent torrent;
            ConnectInfo connectInfo = new ConnectInfo();
            if (saveFileDialogFile.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                doc.Load(openFileDialogTorrent.FileName);
                XmlElement headerElement = doc["share"][ConnectInfo.XmlName];
                XmlElement torrentElement = doc["share"][Torrent.XmlName];
                
                torrent = Torrent.CreateFromXmlShare(torrentElement, saveFileDialogFile.FileName);
                connectInfo = ConnectInfo.ParseXml(headerElement);

                
                addRow(torrent);

                try
                {
                    await manager.ConnectTorrentAsync(torrent, connectInfo);

                }
                catch (SocketException ex)
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

        private void toolStripButtonRemove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in dataGridView.SelectedRows)
            {
                manager.Remove((Torrent)r.Tag);
                dataGridView.Rows.Remove(r);
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            updateTable();
        }

        private void toolStripButtonPause_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in dataGridView.SelectedRows)
            {
                Torrent t = r.Tag as Torrent;
                t?.Pause();
            }
        }
    }
}
