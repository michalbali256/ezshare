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
            this.columnTorrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnProgress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDownload = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnUpload = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.contextMenuStripRow = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveshareFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialogFile = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogShare = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.saveFileDialogFile = new System.Windows.Forms.SaveFileDialog();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
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
            this.columnTorrent,
            this.columnProgress,
            this.columnState,
            this.columnSize,
            this.columnDownload,
            this.columnUpload});
            this.dataGridView.Location = new System.Drawing.Point(12, 68);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(757, 314);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView_MouseDown);
            // 
            // columnTorrent
            // 
            this.columnTorrent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.columnTorrent.HeaderText = "Torrent";
            this.columnTorrent.Name = "columnTorrent";
            this.columnTorrent.ReadOnly = true;
            this.columnTorrent.Width = 141;
            // 
            // columnProgress
            // 
            this.columnProgress.HeaderText = "Progress";
            this.columnProgress.Name = "columnProgress";
            this.columnProgress.ReadOnly = true;
            // 
            // columnState
            // 
            this.columnState.HeaderText = "State";
            this.columnState.Name = "columnState";
            this.columnState.ReadOnly = true;
            // 
            // columnSize
            // 
            this.columnSize.HeaderText = "Size";
            this.columnSize.Name = "columnSize";
            this.columnSize.ReadOnly = true;
            // 
            // columnDownload
            // 
            this.columnDownload.HeaderText = "Download";
            this.columnDownload.Name = "columnDownload";
            this.columnDownload.ReadOnly = true;
            // 
            // columnUpload
            // 
            this.columnUpload.HeaderText = "Upload";
            this.columnUpload.Name = "columnUpload";
            this.columnUpload.ReadOnly = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 488);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(781, 22);
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
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(781, 25);
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
            this.toolStripButtonConnect.Text = "Connect to existing torrent";
            this.toolStripButtonConnect.Click += new System.EventHandler(this.toolStripButtonConnect_Click);
            // 
            // toolStripButtonAdd
            // 
            this.toolStripButtonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAdd.Image = global::miTorrent.Properties.Resources.Custom_Icon_Design_Flatastic_1_Add;
            this.toolStripButtonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAdd.Name = "toolStripButtonAdd";
            this.toolStripButtonAdd.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAdd.Text = "Create torrent from existing file";
            this.toolStripButtonAdd.Click += new System.EventHandler(this.toolStripButtonAdd_Click);
            // 
            // toolStripButtonRemove
            // 
            this.toolStripButtonRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemove.Image = global::miTorrent.Properties.Resources.Awicons_Vista_Artistic_Delete;
            this.toolStripButtonRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemove.Name = "toolStripButtonRemove";
            this.toolStripButtonRemove.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRemove.Text = "Delete torrent";
            this.toolStripButtonRemove.Click += new System.EventHandler(this.toolStripButtonRemove_Click);
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
            this.toolStripButtonStart.Text = "Start";
            this.toolStripButtonStart.Click += new System.EventHandler(this.toolStripButtonStart_Click);
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
            this.saveshareFileToolStripMenuItem.Click += new System.EventHandler(this.saveshareFileToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // openFileDialogFile
            // 
            this.openFileDialogFile.FileName = "openFileDialog1";
            // 
            // saveFileDialogShare
            // 
            this.saveFileDialogShare.Filter = "Share files|*.share|All files|*.*";
            // 
            // menuStrip
            // 
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(781, 24);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "menuStrip1";
            // 
            // listBoxLog
            // 
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.Location = new System.Drawing.Point(12, 388);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(757, 95);
            this.listBoxLog.TabIndex = 5;
            // 
            // timerUpdate
            // 
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(781, 510);
            this.Controls.Add(this.listBoxLog);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.dataGridView);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Main";
            this.Text = "EasyShare";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Main_Load);
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
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRow;
        private System.Windows.Forms.ToolStripMenuItem saveshareFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialogFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnTorrent;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnProgress;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnState;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDownload;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnUpload;
        private System.Windows.Forms.SaveFileDialog saveFileDialogShare;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.SaveFileDialog saveFileDialogFile;
        private System.Windows.Forms.Timer timerUpdate;
    }
}

