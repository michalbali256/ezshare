using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
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
            Torrent t = new Torrent();
            TorrentProperties f = new TorrentProperties(t, true);
            if (f.ShowDialog() != DialogResult.OK)
                return;
            manager.Add(t);
            DataGridViewRow r = new DataGridViewRow();
            r.Tag = t;
            r.ContextMenuStrip = contextMenuStripRow;
            r.CreateCells(dataGridView);
            r.Cells[0].Value = t.File;
            
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
    }
}
