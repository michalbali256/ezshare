using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace miTorrent
{
    public partial class TorrentProperties : Form
    {
        public TorrentProperties(Torrent t, bool creatingNew)
        {
            InitializeComponent();
            editing = t;
            textBoxFileName.Text = t.FilePath;

            if (!creatingNew)
            {
                buttonBrowse.Enabled = false;
                textBoxFileName.ReadOnly = true;
            }
        }

        bool OK = false;
        Torrent editing;

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            openFileDialogFile.FileName = textBoxFileName.Text;
            if (openFileDialogFile.ShowDialog() != DialogResult.OK)
                return;
            textBoxFileName.Text = openFileDialogFile.FileName;

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            OK = true;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TorrentProperties_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (OK)
            {
                DialogResult = DialogResult.OK;
                
            }
            else
                DialogResult = DialogResult.Cancel;
        }
    }
}
