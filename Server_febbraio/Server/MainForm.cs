using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;

namespace Server
{
    public delegate void DisableClipboardCallback(string why);
    public delegate void UpdateUserListCallback(string u);

    public partial class MainForm : Form
    {
        #region INIZIALIZZAZIONE

        private MainServer mainServer;

        // parametri connessione
        private string nickName, password;
        private string helpString = "Start with 'File -> Connect' to set the connection.\r\n" +
                            "To share your screen use 'File -> Start Capture'\r\n" +
                            "You can change the settings in 'Options -> Capture screen settings'\r\n\r\n";
        private IPAddress addr;
        private TcpListener tcpListenerMainServer;
        private int imgport, ipDropdownMenuIndex;
        private int port;
        private bool connectionSettingsOkFlag;
        private static int captureType;
        public bool connected, firstConnection = true;

        public static bool capturing = false;

        public Form connectionSettingsForm;
        // thread aggiuntivo
        public static CaptureWorker workerObject;
        public static Thread workerThread;
        // misure finestra
        private static int w_s, h_s, x_s, y_s;

        private delegate void UpdateStatusCallback(string msg);

        // tasti
        public static Keys kstart;
        public static Keys kstop;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        #endregion

        #region ENTRY POINT

        public MainForm()
        {
            InitializeComponent();

            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
            this.Left = Screen.PrimaryScreen.WorkingArea.Width/2 - this.Width/2;
            this.Top = Screen.PrimaryScreen.WorkingArea.Height/2 - this.Height/2;
            this.setCaptureScreenMeasures(50, 50, 500, 500);
            this.setStartStopKeys(Keys.Up, Keys.Down);

            // per catturare gli eventi anche quando la finestra perde focus...
            _hookID = SetHook(_proc);

            connectionSettingsOkFlag = false;
            captureType = 1;
            workerObject = new CaptureWorker(captureType);
            
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
 
            this.AutoSize = true;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            mainServer = new MainServer(this);
            buttonSend.Enabled = false;
            txtMessage.Enabled = false;
            this.FormClosing += new FormClosingEventHandler(MainForm_Closing);

            this.txtLog.ForeColor = System.Drawing.Color.Green;
            this.txtLog.Font = new Font(txtLog.SelectionFont, FontStyle.Bold);
            this.txtLog.Text = helpString;

            // label = CAPTURE OFF
            labelCapture.ForeColor = Color.Red;
            labelCapture.Text = "CAPTURE: OFF";
        }
        
        #endregion

        #region GETTERS & SETTERS

        public void setStartStopKeys(Keys start, Keys stop)
        {
            kstart = start;
            kstop = stop;
        }
        public bool setUsername(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                MessageBox.Show("Insert a username", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            nickName = user;
            return true;
        }
        public bool setPassword(string psw)
        {
            if (string.IsNullOrEmpty(psw))
            {
                MessageBox.Show("Insert a password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            password = psw;
            return true;
        }
        public bool setIp(string ip)
        {
            try
            {
                addr = IPAddress.Parse(ip);
                return true;
            }
            catch
            {
                MessageBox.Show("Insert a valid Ip address", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public bool setPort(string p)
        {
            try
            {
                port = int.Parse(p);
                if (port <= 1024 || port >= 65536)
                    throw new Exception();

                return true;
            }
            catch
            {
                MessageBox.Show("Insert a valid port (number > 1024 and < 65536)", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public void setConnectionSettingsOkFlag(bool flag)
        {
            connectionSettingsOkFlag = flag;
        }
        public void setCaptureScreenMeasures(int x, int y, int w, int h)
        {
            x_s = x;
            y_s = y;
            w_s = w;
            h_s = h;
        }
        public void setIpDropdownMenuIndex(int p)
        {
            ipDropdownMenuIndex = p;
        }
        public int getCaptureType()
        {
            return captureType;
        }
        public Rectangle getWindowRectangle()
        {
            return new Rectangle(x_s, y_s, w_s, h_s);
        }
        
        public static bool isCapturing() { return capturing; }

        #endregion

        #region HANDLER CHIUSURA/USCITA FORM

        // prima Form Close -> poi Application Exit
        public void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (connected)
            {
                disconnect();
            }
        }
        public void OnApplicationExit(object sender, EventArgs e)
        {
            try
            {
                if (capturing)
                {
                    workerObject.forceStop();
                    workerThread.Join(1000);
                    workerThread.Abort();
                }
            }
            catch { }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnhookWindowsHookEx(_hookID);
            this.Close();
        }
        void connectionSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ConnectionSettingsForm csForm = (ConnectionSettingsForm)sender;
            
            if (connectionSettingsOkFlag && !capturing)
                startStopCaptureToolStripMenuItem.Enabled = true;
        }
        void captureSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CaptureSettingsForm csForm = (CaptureSettingsForm)sender;
            captureType = csForm.getCaptureType();
            _hookID = SetHook(_proc);
        }
        void keyboardSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            KeyboardSettingsForm f = (KeyboardSettingsForm)sender;
            if (f.flag) { UnhookWindowsHookEx(_hookID); _hookID = SetHook(_proc); }
        }

        #endregion

        #region GESTIONE CONNESSIONE

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!connected)
            {
                if (firstConnection && !connectionSettingsOkFlag)
                {
                    // se e' la prima connessione => Autenticazione
                    connectionSettingsForm = new ConnectionSettingsForm(this, false);
                    connectionSettingsForm.ShowDialog();
                    // se si preme ANNULLA (la form non ha settato =1 il flag...)
                    if (!connectionSettingsOkFlag)
                        return;
                }

                // verifica se la porta scelta e' libera...
                IPEndPoint[] tcpConnInfoArray = System.Net.NetworkInformation.IPGlobalProperties.
                                                    GetIPGlobalProperties().GetActiveTcpListeners();
                foreach (IPEndPoint endpoint in tcpConnInfoArray)
                    if (endpoint.Port == port)
                    {
                        System.Windows.Forms.MessageBox.Show("Connection failed: the selected port is already in use", "Error",
                                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                // cerca una porta libera per la condivisione schermo (loop)
                for (imgport = 1025; (imgport < 65535) && (Array.Find(tcpConnInfoArray, comparePort) != null); ++imgport);

                if (firstConnection)
                {
                    // (solo alla prima connessione...)
                    // colleghiamo gli eventi di MainServer con la MainForm in modo che ad ogni arrivo di connessioni/messaggi
                    // la form principale venga informata
                    MainServer.StatusChanged += new StatusChangedEventHandler(mainServer_StatusChanged);
                    txtLog.Clear();
                    firstConnection = false;
                }

                // => CONFIGURO IL SERVER CON I PARAMETRI OTTENUTI!
                mainServer.configureServer(addr, password, port.ToString(), imgport);

                // inizia ad attendere connessioni
                tcpListenerMainServer = mainServer.startListening();

                this.txtLog.Font = new Font(txtLog.SelectionFont, FontStyle.Regular);
                txtLog.SelectionColor = Color.Black;
                txtLog.AppendText("Waiting for connections...\r\n\r\n");
                connettiToolStripMenuItem.Text = "Disconnect";

                startStopCaptureToolStripMenuItem.Enabled = true;

                // label = CAPTURE OFF
                labelCapture.ForeColor = Color.Red;
                labelCapture.Text = "CAPTURE: OFF";

                buttonSend.Enabled = true;
                txtMessage.Enabled = true;
                connected = true;
            }
            else
            {
                stopCapture();

                disconnect();

                connettiToolStripMenuItem.Text = "Connect";
                txtLog.SelectionColor = Color.Black;
                txtLog.AppendText(" ");
                txtLog.SelectionColor = Color.Red;
                txtLog.AppendText("Stopped listening to connections.\r\n\r\n");
                buttonSend.Enabled = false;
                buttonClipboard.Enabled = false;
                txtMessage.Enabled = false;
                startStopCaptureToolStripMenuItem.Enabled = false;
                connected = false;
            }
        }
        private bool comparePort(IPEndPoint p)
        {
            return (p.Port == imgport);
        }
        private void connectionSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // se ho gia' inserito i dati, la finestra Autenticazione viene creata con essi
            if (!connectionSettingsOkFlag)
                connectionSettingsForm = new ConnectionSettingsForm(this, true);
            else
                connectionSettingsForm = new ConnectionSettingsForm(this, ipDropdownMenuIndex, password, port.ToString(), true);
            connectionSettingsForm.Activate();
            connectionSettingsForm.Show();

            connectionSettingsForm.FormClosed += new FormClosedEventHandler(connectionSettingsForm_FormClosed);
        }
        public void updateUsersList(ICollection users)
        {
            this.usersListBox.Items.Clear();
            if (users == null)
            {
                this.usersListBox.Items.Add("<No users>");
                return;
            }

            foreach (string u in users)
                this.usersListBox.Items.Add(u);
        }
        private void disconnect()
        {
            mainServer.setRunning(false);
            // se stiamo spedendo la clipboard...
            mainServer.closeClipboardConnection();
            //mainServer.removeAllUsers();
            mainServer.closeCaptureConnection();

            tcpListenerMainServer.Stop();
        }

        #endregion

        #region GESTIONE CATTURA

        private void startStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (capturing)
            {
                //startStopCaptureToolStripMenuItem.Text = "Start Capture (CTRL+" + kstart.ToString() + ")";
                stopCapture();
            }
            else
            {
                //startStopCaptureToolStripMenuItem.Text = "Stop Capture (CTRL+" + kstop.ToString() + ")";
                startCapture();
            }
        }
        private static void startCapture()
        {
            if (!capturing)
            {
                try
                {
                    workerObject.setScreenParameters(x_s, y_s, w_s, h_s);
                    workerObject.setCaptureType(captureType);
                    workerObject.forceStart();

                    workerThread = new Thread(workerObject.DoWork);
                    workerThread.Start();

                    // label = CAPTURE ON
                    labelCapture.ForeColor = Color.Green;
                    labelCapture.Text = "CAPTURE: ON";

                    while (!workerThread.IsAlive) ; // aspettiamo che si inizializzi
                    capturing = true;
                }
                catch(Exception exc)
                {
                    MessageBox.Show("Error starting capture...\r\n---\r\n"+exc.Message);
                }
            }
        }
        private static void stopCapture()
        {
            if (capturing)
            {
                try
                {
                    workerObject.forceStop();

                    // label = CAPTURE OFF
                    labelCapture.ForeColor = Color.Red;
                    labelCapture.Text = "CAPTURE: OFF";

                    capturing = false;
                }
                catch(Exception exc)
                {
                    MessageBox.Show("Error stopping capture...\r\n---\r\n"+exc.Message);
                }
            }
        }
        
        private void captureSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnhookWindowsHookEx(_hookID);

            // se una cattura è attiva non posso modificarli...
            if (capturing)
                MessageBox.Show("A capture is active. Settings are read-only!", "Warning",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
    
            CaptureSettingsForm csForm = new CaptureSettingsForm(this);
            csForm.Activate();
            csForm.Show();
            //csForm.Left = Screen.PrimaryScreen.Bounds.Width - csForm.Width;
            //csForm.setScreen(x_s,y_s,w_s,h_s);
            
            csForm.FormClosed += new FormClosedEventHandler(captureSettingsForm_FormClosed);
        }

        #endregion

        #region GESTIONE CHAT

        public void mainServer_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            // redirect al metodo di update della form!
            try
            {
                this.Invoke(new UpdateStatusCallback(this.updateLog), new object[] { e.EventMessage });
            }
            catch { }
        }
        private void updateLog(string strMessage)
        {
            // aggiornamento del log:
            // se e' un messaggio di Chat (contiene "USER says: MESSAGGE") coloro USER e MESSAGE
            string search = " says: ";
            int pos = 0;

            // se c'e' gia' un messaggio ne log
            if ((pos = strMessage.IndexOf(search)) != -1)
            {
                string user = strMessage.Substring(0, pos);
                string msg = strMessage.Substring(pos);

                txtLog.SelectionColor = Color.Black;
                txtLog.AppendText("> ");

                txtLog.SelectionColor = Color.Blue;
                txtLog.AppendText(user);
                txtLog.SelectionColor = Color.Black;
                txtLog.AppendText(msg + "\r\n\r\n");
            }
            else // se non e' un messaggio di Chat non coloro
            {
                txtLog.SelectionColor = Color.Black;
                txtLog.AppendText("> ");

                txtLog.SelectionColor = Color.Red;
                this.txtLog.Font = new Font(txtLog.SelectionFont, FontStyle.Regular);
                txtLog.AppendText(strMessage + "\r\n\r\n");
            }
        }
        private void buttonSend_Click(object sender, EventArgs e)
        {
            sendChatMessage();
        }
        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            // per mantenere il testo scrollato fino in fondo
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
            txtLog.Refresh();
        }
        private void txtLog_KeyUp(object sender, KeyEventArgs e)
        {
            // se ho premuto INVIO (ENTER)
            if (e.KeyData == Keys.Enter)
            {
                sendChatMessage();
            }
        }
        private void sendChatMessage()
        {
            string txt = txtMessage.Text.Replace("\r\n", " ");
            if (txtMessage.Lines.Length > 0)
                mainServer.sendUserMessage("Server", txt);

            txtMessage.Text = txtMessage.Text.Replace("\r\n", "");
            txtMessage.Clear();
        }

        #endregion

        #region GESTIONE VOCI MENU'

        private void helpQuestionMarkStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.txtLog.AppendText(helpString);
        }
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // creo dinamicamente il testo del menu File, con le shortcut che ho scelto
            if (capturing)
                startStopCaptureToolStripMenuItem.Text = "Stop Capture (CTRL+" + kstop.ToString() + ")";
            else
                startStopCaptureToolStripMenuItem.Text = "Start Capture (CTRL+" + kstart.ToString() + ")";

            //stopCaptureToolStripMenuItem.Text = "Stop Capture (CTRL+" + kstop.ToString() + ")";
        }
        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dirPath = @mainServer.getSharedFolderLocation();
            string windir = Environment.GetEnvironmentVariable("WINDIR");

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = windir + @"\explorer.exe";
            process.StartInfo.Arguments = dirPath;
            process.Start();
        }

        #endregion

        #region GESTIONE KEYBOARD

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        private void keyboardSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KeyboardSettingsForm formKeys = new KeyboardSettingsForm(this);
            formKeys.Activate();
            formKeys.Show();

            formKeys.FormClosed += new FormClosedEventHandler(keyboardSettingsForm_FormClosed);
        }
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, // Installs a hook procedure that monitors low-level keyboard input events
                                        proc,           // A pointer to the hook procedure.
                                        GetModuleHandle(currentModule.ModuleName), // A handle to the DLL containing the hook procedure
                                        0); // if this parameter is zero, the hook procedure is associated with all existing threads
                                            // running in the same desktop as the calling thread.
            }
        }
        // procedura collegata a LowLevelKeyboardProc
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode >= 0 && wParam == (IntPtr) WM_KEYDOWN)
                {
                    int pressedKey = Marshal.ReadInt32(lParam);

                    // combinazione CTRL + kstart
                    if ((Keys) pressedKey == kstart && Control.ModifierKeys == Keys.Control)
                        startCapture();

                    // combinazione CTRL + kstop
                    if ((Keys)pressedKey == kstop && Control.ModifierKeys == Keys.Control)
                        stopCapture();
                }
            }
            catch { }

            // facciamo procedere oltre la chiamata di sistema
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        #endregion

        #region GESTIONE CLIPBOARD

        /* Adds the specified window to the chain of clipboard viewers.
         * Clipboard viewer windows receive a WM_DRAWCLIPBOARD message whenever the content of the clipboard changes. */
        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        /* Sends the specified message to a window or windows. The sendUserMessage function calls the window procedure
         * for the specified window and does not return until the window procedure has processed the message.*/
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        IntPtr nextClipboardViewer;

        protected override void WndProc(ref System.Windows.Forms.Message msg)
        {
            // "winuser.h"
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (msg.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    if (connected)
                        buttonClipboard.Enabled = true;

                    SendMessage(nextClipboardViewer, msg.Msg, msg.WParam, msg.LParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (msg.WParam == nextClipboardViewer)
                        nextClipboardViewer = msg.LParam;
                    else
                        SendMessage(nextClipboardViewer, msg.Msg, msg.WParam, msg.LParam);
                    break;

                default:
                    base.WndProc(ref msg);
                    break;
            }
        }
        private void buttonClipboard_Click(object sender, EventArgs e)
        {
            string dataText;
            IDataObject data = Clipboard.GetDataObject();

            // se devo inviare TESTO
            if (data.GetDataPresent(DataFormats.Text))
            {
                dataText = (string)data.GetData(DataFormats.Text);
                mainServer.sendClipboardText(dataText);
            }

            // se devo inviare FILE
            else if (data.GetDataPresent(DataFormats.FileDrop, true))
            {
                // creo un thread che lavori in parallelo!
                ParameterizedThreadStart threadSendFile = new ParameterizedThreadStart(mainServer.sendClipboardFile);
                Thread threadClipboard = new Thread(threadSendFile);
                threadClipboard.Start(data);
            }
            // se devo inviare IMMAGINE
            else if (Clipboard.ContainsImage())
            {
                // non creo thread (non conviene, vero?)
                Bitmap img = (Bitmap)Clipboard.GetImage();
                mainServer.sendClipboardImage(img);
            }
        }
        public void disableClipboard(string why)
        {
            buttonClipboard.Enabled = false;
            buttonClipboard.Text = why;
        }
        public void enableClipboard(string why)
        {
            buttonClipboard.Enabled = true;
            buttonClipboard.Text = why;
        }

        #endregion

    }
}