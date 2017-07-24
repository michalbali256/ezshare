namespace miTorrent
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonConnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRemove = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonStart = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPause = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.timerRefreshTable = new System.Windows.Forms.Timer(this.components);
            this.openFileDialogTorrent = new System.Windows.Forms.OpenFileDialog();
            this.Torrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Upload = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripRow = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveshareFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStripRow.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Torrent,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column1,
            this.Upload});
            this.dataGridView.Location = new System.Drawing.Point(12, 68);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(643, 348);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView_MouseDown);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 419);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(667, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonConnect,
            this.toolStripButtonAdd,
            this.toolStripButtonRemove,
            this.toolStripSeparator1,
            this.toolStripButtonStart,
            this.toolStripButtonPause,
            this.toolStripButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(667, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonConnect
            // 
            this.toolStripButtonConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonConnect.Image = global::miTorrent.Properties.Resources._67422_200;
            this.toolStripButtonConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonConnect.Name = "toolStripButtonConnect";
            this.toolStripButtonConnect.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonConnect.Text = "toolStripButton6";
            // 
            // toolStripButtonAdd
            // 
            this.toolStripButtonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAdd.Image = global::miTorrent.Properties.Resources.Custom_Icon_Design_Flatastic_1_Add;
            this.toolStripButtonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAdd.Name = "toolStripButtonAdd";
            this.toolStripButtonAdd.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAdd.Text = "toolStripButton4";
            this.toolStripButtonAdd.Click += new System.EventHandler(this.toolStripButtonAdd_Click);
            // 
            // toolStripButtonRemove
            // 
            this.toolStripButtonRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemove.Image = global::miTorrent.Properties.Resources.Awicons_Vista_Artistic_Delete;
            this.toolStripButtonRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemove.Name = "toolStripButtonRemove";
            this.toolStripButtonRemove.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRemove.Text = "toolStripButton5";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonStart
            // 
            this.toolStripButtonStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStart.Image = global::miTorrent.Properties.Resources.Play1Hot;
            this.toolStripButtonStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStart.Name = "toolStripButtonStart";
            this.toolStripButtonStart.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonStart.Text = "Play";
            // 
            // toolStripButtonPause
            // 
            this.toolStripButtonPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPause.Image = global::miTorrent.Properties.Resources.circled_pause1600;
            this.toolStripButtonPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPause.Name = "toolStripButtonPause";
            this.toolStripButtonPause.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPause.Text = "Pause";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = global::miTorrent.Properties.Resources.Stop;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "Stop";
            // 
            // timerRefreshTable
            // 
            this.timerRefreshTable.Interval = 1000;
            // 
            // openFileDialogTorrent
            // 
            this.openFileDialogTorrent.FileName = "openFileDialog1";
            // 
            // Torrent
            // 
            this.Torrent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Torrent.HeaderText = "Torrent";
            this.Torrent.Name = "Torrent";
            this.Torrent.ReadOnly = true;
            this.Torrent.Width = 141;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Progress";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "State";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Size";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Download";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Upload
            // 
            this.Upload.HeaderText = "Upload";
            this.Upload.Name = "Upload";
            this.Upload.ReadOnly = true;
            // 
            // contextMenuStripRow
            // 
            this.contextMenuStripRow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveshareFileToolStripMenuItem,
            this.propertiesToolStripMenuItem});
            this.contextMenuStripRow.Name = "contextMenuStripRow";
            this.contextMenuStripRow.Size = new System.Drawing.Size(152, 48);
            this.contextMenuStripRow.Opened += new System.EventHandler(this.contextMenuStripRow_Opened);
            // 
            // saveshareFileToolStripMenuItem
            // 
            this.saveshareFileToolStripMenuItem.Name = "saveshareFileToolStripMenuItem";
            this.saveshareFileToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.saveshareFileToolStripMenuItem.Text = "Save .share file";
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 441);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.dataGridView);
            this.Name = "Main";
            this.Text = "EasyShare";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStripRow.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonStart;
        private System.Windows.Forms.ToolStripButton toolStripButtonPause;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdd;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemove;
        private System.Windows.Forms.ToolStripButton toolStripButtonConnect;
        private System.Windows.Forms.Timer timerRefreshTable;
        private System.Windows.Forms.OpenFileDialog openFileDialogTorrent;
        private System.Windows.Forms.DataGridViewTextBoxColumn Torrent;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Upload;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRow;
        private System.Windows.Forms.ToolStripMenuItem saveshareFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
    }
}

