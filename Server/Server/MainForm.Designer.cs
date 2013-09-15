namespace Server
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connettiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startStopCaptureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.opzioniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.captureSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyboardSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpQuestionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.buttonClipboard = new System.Windows.Forms.Button();
            labelCapture = new System.Windows.Forms.Label();
            this.usersListBox = new System.Windows.Forms.ListBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.opzioniToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(570, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connettiToolStripMenuItem,
            this.startStopCaptureToolStripMenuItem,
            this.openFolderToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // connettiToolStripMenuItem
            // 
            this.connettiToolStripMenuItem.Name = "connettiToolStripMenuItem";
            this.connettiToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.connettiToolStripMenuItem.Text = "Connect";
            this.connettiToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // startStopCaptureToolStripMenuItem
            // 
            this.startStopCaptureToolStripMenuItem.Enabled = false;
            this.startStopCaptureToolStripMenuItem.Name = "startStopCaptureToolStripMenuItem";
            this.startStopCaptureToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.startStopCaptureToolStripMenuItem.Text = "Start/Stop Capture";
            this.startStopCaptureToolStripMenuItem.Click += new System.EventHandler(this.startStopToolStripMenuItem_Click);
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.openFolderToolStripMenuItem.Text = "Open Folder";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // opzioniToolStripMenuItem
            // 
            this.opzioniToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionSettingsToolStripMenuItem,
            this.captureSettingsToolStripMenuItem,
            this.keyboardSettingsToolStripMenuItem});
            this.opzioniToolStripMenuItem.Name = "opzioniToolStripMenuItem";
            this.opzioniToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.opzioniToolStripMenuItem.Text = "Settings";
            // 
            // connectionSettingsToolStripMenuItem
            // 
            this.connectionSettingsToolStripMenuItem.Name = "connectionSettingsToolStripMenuItem";
            this.connectionSettingsToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.connectionSettingsToolStripMenuItem.Text = "Connection settings";
            this.connectionSettingsToolStripMenuItem.Click += new System.EventHandler(this.connectionSettingsToolStripMenuItem_Click);
            // 
            // captureSettingsToolStripMenuItem
            // 
            this.captureSettingsToolStripMenuItem.Name = "captureSettingsToolStripMenuItem";
            this.captureSettingsToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.captureSettingsToolStripMenuItem.Text = "Capture screen settings";
            this.captureSettingsToolStripMenuItem.Click += new System.EventHandler(this.captureSettingsToolStripMenuItem_Click);
            // 
            // keyboardSettingsToolStripMenuItem
            // 
            this.keyboardSettingsToolStripMenuItem.Name = "keyboardSettingsToolStripMenuItem";
            this.keyboardSettingsToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.keyboardSettingsToolStripMenuItem.Text = "Keyboard settings";
            this.keyboardSettingsToolStripMenuItem.Click += new System.EventHandler(this.keyboardSettingsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpQuestionToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // helpQuestionToolStripMenuItem
            // 
            this.helpQuestionToolStripMenuItem.Name = "helpQuestionToolStripMenuItem";
            this.helpQuestionToolStripMenuItem.Size = new System.Drawing.Size(90, 22);
            this.helpQuestionToolStripMenuItem.Text = "?";
            this.helpQuestionToolStripMenuItem.Click += new System.EventHandler(this.helpQuestionMarkStripMenuItem2_Click);
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtLog.Location = new System.Drawing.Point(12, 41);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.txtLog.Size = new System.Drawing.Size(536, 324);
            this.txtLog.TabIndex = 2;
            this.txtLog.Text = "";
            this.txtLog.TextChanged += new System.EventHandler(this.txtLog_TextChanged);
            this.txtLog.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtLog_KeyUp);
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(423, 403);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(52, 56);
            this.buttonSend.TabIndex = 4;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(128, 403);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(289, 56);
            this.txtMessage.TabIndex = 5;
            this.txtMessage.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtLog_KeyUp);
            // 
            // buttonClipboard
            // 
            this.buttonClipboard.BackColor = System.Drawing.Color.CadetBlue;
            this.buttonClipboard.Enabled = false;
            this.buttonClipboard.Location = new System.Drawing.Point(481, 403);
            this.buttonClipboard.Name = "buttonClipboard";
            this.buttonClipboard.Size = new System.Drawing.Size(67, 56);
            this.buttonClipboard.TabIndex = 6;
            this.buttonClipboard.Text = "Share Clipboard";
            this.buttonClipboard.UseVisualStyleBackColor = false;
            this.buttonClipboard.Click += new System.EventHandler(this.buttonClipboard_Click);
            // 
            // labelCapture
            // 
            labelCapture.AutoSize = true;
            labelCapture.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelCapture.Location = new System.Drawing.Point(125, 379);
            labelCapture.Name = "labelCapture";
            labelCapture.Size = new System.Drawing.Size(72, 16);
            labelCapture.TabIndex = 7;
            labelCapture.Text = "capture...";
            // 
            // usersListBox
            // 
            this.usersListBox.Enabled = false;
            this.usersListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usersListBox.FormattingEnabled = true;
            this.usersListBox.Items.AddRange(new object[] {
            "<No users>"});
            this.usersListBox.Location = new System.Drawing.Point(12, 403);
            this.usersListBox.Name = "usersListBox";
            this.usersListBox.Size = new System.Drawing.Size(107, 56);
            this.usersListBox.TabIndex = 9;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(570, 471);
            this.Controls.Add(this.usersListBox);
            this.Controls.Add(labelCapture);
            this.Controls.Add(this.buttonClipboard);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.menuStrip1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Sharing Server";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem opzioniToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpQuestionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connettiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem captureSettingsToolStripMenuItem;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.ToolStripMenuItem connectionSettingsToolStripMenuItem;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button buttonClipboard;
        private System.Windows.Forms.ToolStripMenuItem keyboardSettingsToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem startStopCaptureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        public System.Windows.Forms.ListBox usersListBox;
        public static System.Windows.Forms.Label labelCapture;

        /* N.B. la label di Capture devono essere STATICI, bisogna riscriverlo ad ogni modifica fatta dal Designer.
         * In più, nel codice autogenerato togliere "this." per i campi statici...
         * Find/Replace:
         *      labelCapture
         */
    }
}