namespace Client
{
    partial class MainForm
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
            this.buttonSend = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.impostazioniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.esciToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBoxCapture = new System.Windows.Forms.PictureBox();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.buttonToggleVideo = new System.Windows.Forms.Button();
            this.buttonToggleChat = new System.Windows.Forms.Button();
            this.labelVideoWarning = new System.Windows.Forms.Label();
            this.buttonClipboard = new System.Windows.Forms.Button();
            this.buttonConnection = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCapture)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSend
            // 
            this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSend.Enabled = false;
            this.buttonSend.Location = new System.Drawing.Point(819, 501);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(67, 49);
            this.buttonSend.TabIndex = 3;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.Enabled = false;
            this.txtMessage.Location = new System.Drawing.Point(566, 501);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(247, 49);
            this.txtMessage.TabIndex = 4;
            this.txtMessage.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtMessage_KeyUp);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(928, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.impostazioniToolStripMenuItem,
            this.openFolderToolStripMenuItem,
            this.esciToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // impostazioniToolStripMenuItem
            // 
            this.impostazioniToolStripMenuItem.Name = "impostazioniToolStripMenuItem";
            this.impostazioniToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.impostazioniToolStripMenuItem.Text = "Settings";
            this.impostazioniToolStripMenuItem.Click += new System.EventHandler(this.connectionSettingsToolStripMenuItem_Click);
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.openFolderToolStripMenuItem.Text = "Open folder";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // esciToolStripMenuItem
            // 
            this.esciToolStripMenuItem.Name = "esciToolStripMenuItem";
            this.esciToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.esciToolStripMenuItem.Text = "Exit";
            this.esciToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(90, 22);
            this.toolStripMenuItem2.Text = "?";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.helpQuestionMarkToolStripMenuItem_Click);
            // 
            // pictureBoxCapture
            // 
            this.pictureBoxCapture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxCapture.BackColor = System.Drawing.Color.DimGray;
            this.pictureBoxCapture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxCapture.Location = new System.Drawing.Point(6, 95);
            this.pictureBoxCapture.Name = "pictureBoxCapture";
            this.pictureBoxCapture.Size = new System.Drawing.Size(520, 450);
            this.pictureBoxCapture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxCapture.TabIndex = 9;
            this.pictureBoxCapture.TabStop = false;
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(566, 39);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(320, 424);
            this.txtLog.TabIndex = 10;
            this.txtLog.Text = "";
            this.txtLog.TextChanged += new System.EventHandler(this.txtLog_TextChanged);
            // 
            // buttonToggleVideo
            // 
            this.buttonToggleVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonToggleVideo.Location = new System.Drawing.Point(100, 39);
            this.buttonToggleVideo.Name = "buttonToggleVideo";
            this.buttonToggleVideo.Size = new System.Drawing.Size(60, 50);
            this.buttonToggleVideo.TabIndex = 11;
            this.buttonToggleVideo.Text = "Toggle Video";
            this.buttonToggleVideo.UseVisualStyleBackColor = true;
            this.buttonToggleVideo.Click += new System.EventHandler(this.toggleVideo_Click);
            // 
            // buttonToggleChat
            // 
            this.buttonToggleChat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonToggleChat.Location = new System.Drawing.Point(166, 39);
            this.buttonToggleChat.Name = "buttonToggleChat";
            this.buttonToggleChat.Size = new System.Drawing.Size(60, 50);
            this.buttonToggleChat.TabIndex = 15;
            this.buttonToggleChat.Text = "Toggle Chat";
            this.buttonToggleChat.UseVisualStyleBackColor = true;
            this.buttonToggleChat.Click += new System.EventHandler(this.toggleChat_Click);
            // 
            // labelVideoWarning
            // 
            this.labelVideoWarning.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelVideoWarning.AutoSize = true;
            this.labelVideoWarning.BackColor = System.Drawing.Color.DimGray;
            this.labelVideoWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVideoWarning.ForeColor = System.Drawing.Color.Black;
            this.labelVideoWarning.Location = new System.Drawing.Point(140, 250);
            this.labelVideoWarning.Name = "labelVideoWarning";
            this.labelVideoWarning.Size = new System.Drawing.Size(244, 31);
            this.labelVideoWarning.TabIndex = 13;
            this.labelVideoWarning.Text = "Video unavailable";
            // 
            // buttonClipboard
            // 
            this.buttonClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClipboard.BackColor = System.Drawing.Color.CadetBlue;
            this.buttonClipboard.Enabled = false;
            this.buttonClipboard.Location = new System.Drawing.Point(634, 469);
            this.buttonClipboard.Name = "buttonClipboard";
            this.buttonClipboard.Size = new System.Drawing.Size(196, 26);
            this.buttonClipboard.TabIndex = 14;
            this.buttonClipboard.Text = "Share Clipboard";
            this.buttonClipboard.UseVisualStyleBackColor = false;
            this.buttonClipboard.Click += new System.EventHandler(this.buttonClipboard_Click);
            // 
            // buttonConnection
            // 
            this.buttonConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConnection.BackColor = System.Drawing.Color.LightGreen;
            this.buttonConnection.Location = new System.Drawing.Point(6, 39);
            this.buttonConnection.Name = "buttonConnection";
            this.buttonConnection.Size = new System.Drawing.Size(88, 49);
            this.buttonConnection.TabIndex = 15;
            this.buttonConnection.Text = "Connect";
            this.buttonConnection.UseVisualStyleBackColor = false;
            this.buttonConnection.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 562);
            this.Controls.Add(this.buttonConnection);
            this.Controls.Add(this.buttonClipboard);
            this.Controls.Add(this.labelVideoWarning);
            this.Controls.Add(this.buttonToggleChat);
            this.Controls.Add(this.buttonToggleVideo);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.pictureBoxCapture);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Sharing Client";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCapture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem impostazioniToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem esciToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBoxCapture;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Button buttonToggleVideo;
        private System.Windows.Forms.Button buttonToggleChat;
        private System.Windows.Forms.Label labelVideoWarning;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.Button buttonClipboard;
        private System.Windows.Forms.Button buttonConnection;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
    }
}

