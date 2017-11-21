using System;
using System.Drawing;
using System.Windows.Forms;
using EzShare.ModelLib;


namespace EzShare
{
    namespace GUI
    {
        /// <summary>
        /// Form for viewing properties of torrent
        /// </summary>
        public partial class TorrentProperties : Form
        {
            private bool OK = false;
            /// <summary>
            /// Torrent that is being edited
            /// </summary>
            private Torrent editing;
            private ProgressViewer progressViewer;

            /// <summary>
            /// Creates new form that edits specified torrent
            /// </summary>
            /// <param name="t"></param>
            /// <param name="creatingNew"></param>
            public TorrentProperties(Torrent t, bool creatingNew)
            {
                InitializeComponent();
                progressViewer = new ProgressViewer(t.File)
                {
                    Location = new Point(56, 73),
                    Size = new Size(504, 20)
                };
                Controls.Add(progressViewer);


                editing = t;
                textBoxFileName.Text = t.FilePath;
                textBoxName.Text = t.Name;
                labelStatusValue.Text = t.Status.ToString();

                if (!creatingNew)
                {
                    textBoxFileName.ReadOnly = true;
                }
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

            /// <summary>
            /// Starts timer for progressViewer update
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void TorrentProperties_Load(object sender, EventArgs e)
            {
                timerUpdate.Enabled = true;
            }

            /// <summary>
            /// Updates progressViewer
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void timerUpdate_Tick(object sender, EventArgs e)
            {
                progressViewer.Refresh();
            }
        }
    }
}