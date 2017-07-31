using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Xml;

namespace miTorrent
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            dataGridView.Rows.Clear();
        }

        TorrentManager manager = new TorrentManager();
        

        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            if (openFileDialogFile.ShowDialog() != DialogResult.OK)
                return;
            Torrent t = Torrent.CreateFromPath(openFileDialogFile.FileName);
            manager.Add(t);
            DataGridViewRow r = new DataGridViewRow();
            r.Tag = t;
            r.ContextMenuStrip = contextMenuStripRow;
            r.CreateCells(dataGridView);
            r.Cells[0].Value = t.FilePath;
            
            dataGridView.Rows.Add(r);
            
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {

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
            documentElement.AppendChild(t.SaveToXml(doc));

            doc.AppendChild(documentElement);
            doc.Save(saveFileDialogShare.FileName);
        }

        private void toolStripButtonStart_Click(object sender, EventArgs e)
        {
            manager.StartListening();
        }

        private void toolStripButtonConnect_Click(object sender, EventArgs e)
        {
            if (openFileDialogTorrent.ShowDialog() != DialogResult.OK)
                return;
            XmlDocument doc = new XmlDocument();
            Torrent torrent;
            ConnectInfo connectInfo;
            try
            {
                doc.Load(openFileDialogTorrent.FileName);
                XmlElement headerElement = doc["share"][ConnectInfo.XmlName];
                XmlElement torrentElement = doc["share"][Torrent.XmlName];
                torrent = Torrent.CreateFromXml(torrentElement);
                connectInfo = ConnectInfo.ParseXml(headerElement);
                
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show("Couldn't open file. " + ex.ToString());
            }
            catch (XmlException ex)
            {
                MessageBox.Show("File has wrong format. " + ex.ToString());
            }
            


        }
    }
}
