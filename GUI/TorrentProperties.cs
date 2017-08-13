using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EzShare.ModelLib;


namespace EzShare
{
    namespace GUI
    {
        public partial class TorrentProperties : Form
        {
            /// <summary>
            /// Creates new form that edits specified torrent
            /// </summary>
            /// <param name="t"></param>
            /// <param name="creatingNew"></param>
            public TorrentProperties(Torrent t, bool creatingNew)
            {
                InitializeComponent();
                progressViewer = new ProgressViewer(t.File);
                progressViewer.Location = new Point(56, 73);
                progressViewer.Size = new Size(504, 20);
                this.Controls.Add(progressViewer);


                editing = t;
                textBoxFileName.Text = t.FilePath;
                textBoxName.Text = t.Name;
                labelStatusValue.Text = t.Status.ToString();

                if (!creatingNew)
                {
                    buttonBrowse.Enabled = false;
                    textBoxFileName.ReadOnly = true;
                }
            }

            ProgressViewer progressViewer;

            bool OK = false;
            /// <summary>
            /// Torrent that is being edited
            /// </summary>
            Torrent editing;

            /// <summary>
            /// Opens file dialog and edits text box.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void buttonBrowse_Click(object sender, EventArgs e)
            {
                openFileDialogFile.FileName = textBoxFileName.Text;
                if (openFileDialogFile.ShowDialog() != DialogResult.OK)
                    return;
                textBoxFileName.Text = openFileDialogFile.FileName;

            }

            /// <summary>
            /// Closes form and sets OK flag
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void buttonOK_Click(object sender, EventArgs e)
            {
                OK = true;
                Close();
            }

            /// <summary>
            /// Just closes form
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void buttonCancel_Click(object sender, EventArgs e)
            {
                Close();
            }

            /// <summary>
            /// If OK flag is set, returns dialog result OK
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void TorrentProperties_FormClosed(object sender, FormClosedEventArgs e)
            {
                if (OK)
                {
                    DialogResult = DialogResult.OK;
                    editing.Name = textBoxName.Text;
                }
                else
                    DialogResult = DialogResult.Cancel;
            }

            private void TorrentProperties_Load(object sender, EventArgs e)
            {
                timerUpdate.Enabled = true;
            }

            private void timerUpdate_Tick(object sender, EventArgs e)
            {
                progressViewer.Refresh();
            }
        }
    }
}