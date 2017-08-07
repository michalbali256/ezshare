namespace EzShare
{
    namespace GUI
    {
        partial class TorrentProperties
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
                this.labelFile = new System.Windows.Forms.Label();
                this.textBoxFileName = new System.Windows.Forms.TextBox();
                this.buttonBrowse = new System.Windows.Forms.Button();
                this.buttonOK = new System.Windows.Forms.Button();
                this.buttonCancel = new System.Windows.Forms.Button();
                this.openFileDialogFile = new System.Windows.Forms.OpenFileDialog();
                this.labelName = new System.Windows.Forms.Label();
                this.textBoxName = new System.Windows.Forms.TextBox();
                this.SuspendLayout();
                // 
                // labelFile
                // 
                this.labelFile.AutoSize = true;
                this.labelFile.Location = new System.Drawing.Point(12, 32);
                this.labelFile.Name = "labelFile";
                this.labelFile.Size = new System.Drawing.Size(26, 13);
                this.labelFile.TabIndex = 0;
                this.labelFile.Text = "File:";
                // 
                // textBoxFileName
                // 
                this.textBoxFileName.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                this.textBoxFileName.Location = new System.Drawing.Point(44, 29);
                this.textBoxFileName.Name = "textBoxFileName";
                this.textBoxFileName.Size = new System.Drawing.Size(389, 20);
                this.textBoxFileName.TabIndex = 1;
                // 
                // buttonBrowse
                // 
                this.buttonBrowse.Location = new System.Drawing.Point(439, 27);
                this.buttonBrowse.Name = "buttonBrowse";
                this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
                this.buttonBrowse.TabIndex = 2;
                this.buttonBrowse.Text = "Browse";
                this.buttonBrowse.UseVisualStyleBackColor = true;
                this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
                // 
                // buttonOK
                // 
                this.buttonOK.Location = new System.Drawing.Point(295, 167);
                this.buttonOK.Name = "buttonOK";
                this.buttonOK.Size = new System.Drawing.Size(75, 23);
                this.buttonOK.TabIndex = 3;
                this.buttonOK.Text = "OK";
                this.buttonOK.UseVisualStyleBackColor = true;
                this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
                // 
                // buttonCancel
                // 
                this.buttonCancel.Location = new System.Drawing.Point(376, 167);
                this.buttonCancel.Name = "buttonCancel";
                this.buttonCancel.Size = new System.Drawing.Size(75, 23);
                this.buttonCancel.TabIndex = 4;
                this.buttonCancel.Text = "Cancel";
                this.buttonCancel.UseVisualStyleBackColor = true;
                this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
                // 
                // openFileDialogFile
                // 
                this.openFileDialogFile.FileName = "openFileDialog1";
                // 
                // labelName
                // 
                this.labelName.AutoSize = true;
                this.labelName.Location = new System.Drawing.Point(0, 9);
                this.labelName.Name = "labelName";
                this.labelName.Size = new System.Drawing.Size(38, 13);
                this.labelName.TabIndex = 5;
                this.labelName.Text = "Name:";
                // 
                // textBoxName
                // 
                this.textBoxName.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                this.textBoxName.Location = new System.Drawing.Point(44, 6);
                this.textBoxName.Name = "textBoxName";
                this.textBoxName.Size = new System.Drawing.Size(389, 20);
                this.textBoxName.TabIndex = 6;
                // 
                // TorrentProperties
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(535, 214);
                this.Controls.Add(this.textBoxName);
                this.Controls.Add(this.labelName);
                this.Controls.Add(this.buttonCancel);
                this.Controls.Add(this.buttonOK);
                this.Controls.Add(this.buttonBrowse);
                this.Controls.Add(this.textBoxFileName);
                this.Controls.Add(this.labelFile);
                this.Name = "TorrentProperties";
                this.Text = "TorrentProperties";
                this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TorrentProperties_FormClosed);
                this.ResumeLayout(false);
                this.PerformLayout();

            }

            #endregion

            private System.Windows.Forms.Label labelFile;
            private System.Windows.Forms.TextBox textBoxFileName;
            private System.Windows.Forms.Button buttonBrowse;
            private System.Windows.Forms.Button buttonOK;
            private System.Windows.Forms.Button buttonCancel;
            private System.Windows.Forms.OpenFileDialog openFileDialogFile;
            private System.Windows.Forms.Label labelName;
            private System.Windows.Forms.TextBox textBoxName;
        }
    }
}
