namespace EzShare
{
    namespace GUI
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
            this.columnConnectedClients = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnBlank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonConnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRemove = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonStart = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPause = new System.Windows.Forms.ToolStripButton();
            this.timerRefreshTable = new System.Windows.Forms.Timer(this.components);
            this.openFileDialogTorrent = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStripRow = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveshareFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this.reconnectDownloadingClientsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialogFile = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogShare = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.torrentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadshareFileAndConnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createNewTorrentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveshareFilesOfAllTorrentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearAllTorrentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.startAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.saveFileDialogFile = new System.Windows.Forms.SaveFileDialog();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStripRow.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnTorrent,
            this.columnProgress,
            this.columnState,
            this.columnSize,
            this.columnDownload,
            this.columnUpload,
            this.columnConnectedClients,
            this.columnBlank});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(908, 261);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyDown);
            this.dataGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DataGridView_MouseDown);
            // 
            // columnTorrent
            // 
            this.columnTorrent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.columnTorrent.HeaderText = "Torrent";
            this.columnTorrent.Name = "columnTorrent";
            this.columnTorrent.ReadOnly = true;
            this.columnTorrent.Width = 250;
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
            // columnConnectedClients
            // 
            this.columnConnectedClients.HeaderText = "ConnectedClients";
            this.columnConnectedClients.Name = "columnConnectedClients";
            this.columnConnectedClients.ReadOnly = true;
            // 
            // columnBlank
            // 
            this.columnBlank.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnBlank.HeaderText = "";
            this.columnBlank.MinimumWidth = 2;
            this.columnBlank.Name = "columnBlank";
            this.columnBlank.ReadOnly = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 469);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(908, 22);
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
            this.toolStripButtonPause});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(908, 25);
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
            this.toolStripButtonConnect.Click += new System.EventHandler(this.ToolStripButtonConnect_Click);
            // 
            // toolStripButtonAdd
            // 
            this.toolStripButtonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAdd.Image = global::miTorrent.Properties.Resources.Custom_Icon_Design_Flatastic_1_Add;
            this.toolStripButtonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAdd.Name = "toolStripButtonAdd";
            this.toolStripButtonAdd.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAdd.Text = "Create torrent from existing file";
            this.toolStripButtonAdd.Click += new System.EventHandler(this.ToolStripButtonAdd_Click);
            // 
            // toolStripButtonRemove
            // 
            this.toolStripButtonRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemove.Image = global::miTorrent.Properties.Resources.Awicons_Vista_Artistic_Delete;
            this.toolStripButtonRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemove.Name = "toolStripButtonRemove";
            this.toolStripButtonRemove.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRemove.Text = "Delete torrent";
            this.toolStripButtonRemove.Click += new System.EventHandler(this.ToolStripButtonRemove_Click);
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
            this.toolStripButtonStart.Click += new System.EventHandler(this.ToolStripButtonStart_Click);
            // 
            // toolStripButtonPause
            // 
            this.toolStripButtonPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPause.Image = global::miTorrent.Properties.Resources.circled_pause1600;
            this.toolStripButtonPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPause.Name = "toolStripButtonPause";
            this.toolStripButtonPause.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPause.Text = "Pause";
            this.toolStripButtonPause.Click += new System.EventHandler(this.ToolStripButtonPause_Click);
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
            this.toolStripMenuItem5,
            this.startToolStripMenuItem,
            this.pauseToolStripMenuItem,
            this.toolStripMenuItem7,
            this.reconnectDownloadingClientsToolStripMenuItem,
            this.toolStripMenuItem6,
            this.propertiesToolStripMenuItem});
            this.contextMenuStripRow.Name = "contextMenuStripRow";
            this.contextMenuStripRow.Size = new System.Drawing.Size(241, 154);
            this.contextMenuStripRow.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripRow_Opening);
            // 
            // saveshareFileToolStripMenuItem
            // 
            this.saveshareFileToolStripMenuItem.Name = "saveshareFileToolStripMenuItem";
            this.saveshareFileToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.saveshareFileToolStripMenuItem.Text = "Save .share file";
            this.saveshareFileToolStripMenuItem.Click += new System.EventHandler(this.SaveshareFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(237, 6);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.StartToolStripMenuItem_Click);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.pauseToolStripMenuItem.Text = "Pause";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.PauseToolStripMenuItem_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(237, 6);
            // 
            // reconnectDownloadingClientsToolStripMenuItem
            // 
            this.reconnectDownloadingClientsToolStripMenuItem.Name = "reconnectDownloadingClientsToolStripMenuItem";
            this.reconnectDownloadingClientsToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.reconnectDownloadingClientsToolStripMenuItem.Text = "Reconnect downloading clients";
            this.reconnectDownloadingClientsToolStripMenuItem.Click += new System.EventHandler(this.ReconnectAllClientsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(237, 6);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.PropertiesToolStripMenuItem_Click);
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
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.torrentsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(908, 24);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "menuStrip1";
            // 
            // torrentsToolStripMenuItem
            // 
            this.torrentsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadshareFileAndConnectToolStripMenuItem,
            this.createNewTorrentToolStripMenuItem,
            this.saveshareFilesOfAllTorrentsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.clearAllTorrentsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.startAllToolStripMenuItem,
            this.pauseAllToolStripMenuItem,
            this.toolStripMenuItem3,
            this.aboutToolStripMenuItem,
            this.toolStripMenuItem4,
            this.exitToolStripMenuItem});
            this.torrentsToolStripMenuItem.Name = "torrentsToolStripMenuItem";
            this.torrentsToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.torrentsToolStripMenuItem.Text = "Torrents";
            // 
            // loadshareFileAndConnectToolStripMenuItem
            // 
            this.loadshareFileAndConnectToolStripMenuItem.Name = "loadshareFileAndConnectToolStripMenuItem";
            this.loadshareFileAndConnectToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.loadshareFileAndConnectToolStripMenuItem.Text = "Load .share file and connect";
            this.loadshareFileAndConnectToolStripMenuItem.Click += new System.EventHandler(this.ToolStripButtonConnect_Click);
            // 
            // createNewTorrentToolStripMenuItem
            // 
            this.createNewTorrentToolStripMenuItem.Name = "createNewTorrentToolStripMenuItem";
            this.createNewTorrentToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.createNewTorrentToolStripMenuItem.Text = "Create new torrent";
            this.createNewTorrentToolStripMenuItem.Click += new System.EventHandler(this.ToolStripButtonAdd_Click);
            // 
            // saveshareFilesOfAllTorrentsToolStripMenuItem
            // 
            this.saveshareFilesOfAllTorrentsToolStripMenuItem.Name = "saveshareFilesOfAllTorrentsToolStripMenuItem";
            this.saveshareFilesOfAllTorrentsToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.saveshareFilesOfAllTorrentsToolStripMenuItem.Text = "Save .share files of all torrents";
            this.saveshareFilesOfAllTorrentsToolStripMenuItem.Click += new System.EventHandler(this.SaveshareFilesOfAllTorrentsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(226, 6);
            // 
            // clearAllTorrentsToolStripMenuItem
            // 
            this.clearAllTorrentsToolStripMenuItem.Name = "clearAllTorrentsToolStripMenuItem";
            this.clearAllTorrentsToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.clearAllTorrentsToolStripMenuItem.Text = "Clear all torrents";
            this.clearAllTorrentsToolStripMenuItem.Click += new System.EventHandler(this.ClearAllTorrentsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(226, 6);
            // 
            // startAllToolStripMenuItem
            // 
            this.startAllToolStripMenuItem.Name = "startAllToolStripMenuItem";
            this.startAllToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.startAllToolStripMenuItem.Text = "Start all";
            this.startAllToolStripMenuItem.Click += new System.EventHandler(this.StartAllToolStripMenuItem_Click);
            // 
            // pauseAllToolStripMenuItem
            // 
            this.pauseAllToolStripMenuItem.Name = "pauseAllToolStripMenuItem";
            this.pauseAllToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.pauseAllToolStripMenuItem.Text = "Pause all";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(226, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(226, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // listBoxLog
            // 
            this.listBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.Location = new System.Drawing.Point(0, 0);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(908, 155);
            this.listBoxLog.TabIndex = 5;
            // 
            // timerUpdate
            // 
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 49);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listBoxLog);
            this.splitContainer1.Size = new System.Drawing.Size(908, 420);
            this.splitContainer1.SplitterDistance = 261;
            this.splitContainer1.TabIndex = 6;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 491);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Main";
            this.Text = "ezShare";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStripRow.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

            }

            #endregion

            private System.Windows.Forms.DataGridView dataGridView;
            private System.Windows.Forms.StatusStrip statusStrip1;
            private System.Windows.Forms.ToolStrip toolStrip1;
            private System.Windows.Forms.ToolStripButton toolStripButtonStart;
            private System.Windows.Forms.ToolStripButton toolStripButtonPause;
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
            private System.Windows.Forms.SaveFileDialog saveFileDialogShare;
            private System.Windows.Forms.MenuStrip menuStrip;
            private System.Windows.Forms.ListBox listBoxLog;
            private System.Windows.Forms.SaveFileDialog saveFileDialogFile;
            private System.Windows.Forms.Timer timerUpdate;
            private System.Windows.Forms.SplitContainer splitContainer1;
            private System.Windows.Forms.DataGridViewTextBoxColumn columnTorrent;
            private System.Windows.Forms.DataGridViewTextBoxColumn columnProgress;
            private System.Windows.Forms.DataGridViewTextBoxColumn columnState;
            private System.Windows.Forms.DataGridViewTextBoxColumn columnSize;
            private System.Windows.Forms.DataGridViewTextBoxColumn columnDownload;
            private System.Windows.Forms.DataGridViewTextBoxColumn columnUpload;
            private System.Windows.Forms.DataGridViewTextBoxColumn columnConnectedClients;
            private System.Windows.Forms.DataGridViewTextBoxColumn columnBlank;
            private System.Windows.Forms.ToolStripMenuItem torrentsToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem loadshareFileAndConnectToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem createNewTorrentToolStripMenuItem;
            private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
            private System.Windows.Forms.ToolStripMenuItem clearAllTorrentsToolStripMenuItem;
            private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
            private System.Windows.Forms.ToolStripMenuItem startAllToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem pauseAllToolStripMenuItem;
            private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
            private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem saveshareFilesOfAllTorrentsToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
            private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
            private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
            private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
            private System.Windows.Forms.ToolStripMenuItem reconnectDownloadingClientsToolStripMenuItem;
            private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
            private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
            private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
        }
    }
}
