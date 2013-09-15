using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;


namespace Client
{
    public delegate void updateImage(Bitmap btmp, bool available);
    public partial class MainForm : Form
    {
        #region INIZIALIZZAZIONE

        private string username = "", password = "";
        private int port = 0, port_capture;
        private bool connected, show_video = true, show_chat = true, first = true, connectionSettingsOkFlag = false;

        private StreamWriter streamSender;
        private StreamReader streamReceiver;
        private StreamReader streamReceiverClipboard; // per clipboard
        private TcpClient tcpServer;
        private TcpClient tcpClipboard;
        
        // delegate per invocare metodi da altri thread
        private delegate void UpdateLogCallback(string why);
        private delegate void CloseConnectionCallback(string why);
        private delegate void DisableClipboardCallback(string why);
        private delegate void UpdateClipboardCallback(System.Collections.Specialized.StringCollection paths);

        private Thread threadChat;
        private IPAddress ipAddress;
        public Form connectionSettingsForm;
        public CaptureSocket captureSocket;

        CaptureWorker workerObject;
        Thread workerThread;

        private const string disconnectionMessage = "BYE";
        private Color buttonConnection_originalBackColor;

        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove,IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)] public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        IntPtr nextClipboardViewer;

        private int previous_width;
        private const string helpString = "Use the 'Connect' button to connect to the Server.\r\n" +
                                "You can change the settings using 'File -> Settings'\r\n\r\n";

        private const string sharedFolderLocation = ".\\Shared Files";

        #endregion

        #region MAIN FORM

        public MainForm()
        {
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);

            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.txtLog.Text = helpString;
            labelVideoWarningCenter();

            buttonConnection_originalBackColor = buttonConnection.BackColor;

            if (!Directory.Exists(@sharedFolderLocation))
                Directory.CreateDirectory(@sharedFolderLocation);
        }
        public void OnApplicationExit(object sender, EventArgs e)
        {
            //ChangeClipboardChain(this.Handle, nextClipboardViewer);
            if (connected)
            {
                disconnect();
            }
        }

        #endregion

        #region GETTERS & SETTERS

        public bool setUsername(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                MessageBox.Show("Username is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Match match = Regex.Match(user, "^[A-Za-z0-9_-]{3,16}$", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                MessageBox.Show("Username is not valid: only letters, numbers '-' and '_' are allowed.\r\n"
                                + "Min: 3 digits, Max: 16 digits", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            username = user;
            return true;
        }
        public bool setPassword(string psw)
        {
            if (string.IsNullOrEmpty(psw))
            {
                MessageBox.Show("Password is not valid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            password = psw;
            return true;
        }
        public bool setIp(string ip)
        {
            try
            {
                ipAddress = IPAddress.Parse(ip);
                return true;
            }
            catch
            {
                MessageBox.Show("IP address is not correct", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        public void setConnectionSettingsOkFlag()
        {
            connectionSettingsOkFlag = true;
        }

        #endregion

        #region GESTIONE CATTURA

        public void updateImage(Bitmap bmp, bool available)
        {
            if (workerObject.isStopped())
                return;

            if (available)
            {
                labelVideoWarning.Visible = false;
                pictureBoxCapture.Image = bmp;
                pictureBoxCapture.BackColor = Color.Beige;
            }
            else
            {
                if(show_video)
                    labelVideoWarning.Visible = true;
                pictureBoxCapture.Image = null;
                pictureBoxCapture.BackColor = Color.DimGray;
            }

            pictureBoxCapture.Refresh();
        }
        private void labelVideoWarningCenter()
        {
            this.labelVideoWarning.Top = pictureBoxCapture.Top + ((pictureBoxCapture.Height - this.labelVideoWarning.Height) / 2);
            this.labelVideoWarning.Left = pictureBoxCapture.Left + (pictureBoxCapture.Width - this.labelVideoWarning.Width) / 2;
        }

        #endregion

        #region GESTIONE CONNESSIONE

        private void initializeConnection()
        {
            // spedisco md5 della password specificata
            MD5 md5 = new MD5CryptoServiceProvider();
            string password_md5 = BitConverter.ToString(md5.ComputeHash(ASCIIEncoding.Default.GetBytes(password)));

            // prima di mandare, verifico che la porta scelta sia libera
            /* NON USARE IN LOCALE! ...usiamo la stessa porta per Server e Client 
            IPEndPoint[] tcpConnInfoArray = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            foreach (IPEndPoint endpoint in tcpConnInfoArray)
                if (endpoint.Port == port)
                {
                    System.Windows.Forms.MessageBox.Show("Connection failed: the selected port is already in use", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            */

            // connessione...
            tcpServer = new TcpClient();
            try
            {
                tcpServer.Connect(new IPEndPoint(ipAddress, port));
                
                connected = true;
                txtMessage.Enabled = true;
                buttonSend.Enabled = true;
                buttonConnection.Text = "Disconnect";
                buttonConnection.BackColor = Color.Red;
                if (first)
                {
                    txtLog.Clear();
                    first = false;
                }

                // spedisco user + password
                streamSender = new StreamWriter(tcpServer.GetStream());
                streamSender.WriteLine(username);
                streamSender.Flush();
                streamSender.WriteLine(password_md5);
                streamSender.Flush();

                // usiamo un thread esclusivamente per la Chat!
                threadChat = new Thread(new ThreadStart(receiveChatMessage));
                threadChat.Start();
            }
            catch(Exception e)
            {
                txtLog.AppendText("Error connecting to Server ("+ ipAddress +"):\r\n"+ e.Message+"\r\n\r\n");
                //buttonConnection.Text = "Connect";
                //buttonConnection.BackColor = buttonConnection_originalBackColor;
            }
        }
        private void disconnect()
        {
            try
            {
                string text = disconnectionMessage;
                streamSender.WriteLine(text);
                streamSender.Flush();

                streamSender.Close();
                streamReceiver.Close();
                tcpServer.Close();

                streamReceiverClipboard.Close();
                tcpClipboard.Close();

            }
            catch (Exception exc)
            {
                txtLog.AppendText("Error disconnecting from Server:\n" + exc.Message);
            }

            connected = false;

            try
            {
                workerObject.forceStop();
                workerThread.Join(2000); // aspettiamo
                workerThread.Abort(); // se no, uscita sporca...
            }
            catch (Exception exc)
            {
               // MessageBox.Show("Errore worker:\n" + exc.Message);
            }
        }
        private void closeConnection(string why)
        {
            disconnect();

            txtLog.SelectionColor = Color.Black;
            txtLog.AppendText("> ");

            txtLog.SelectionColor = Color.Red;
            txtLog.AppendText("Disconnected: " + why + "\r\n\n");

            txtMessage.Enabled = false;
            buttonSend.Enabled = false;
            buttonConnection.Text = "Connect";
            buttonConnection.BackColor = buttonConnection_originalBackColor;
            buttonClipboard.Enabled = false;

            if (show_video)
                labelVideoWarning.Visible = true;
            pictureBoxCapture.BackColor = Color.DimGray;

            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Height, Screen.PrimaryScreen.Bounds.Width);
            pictureBoxCapture.Image = bmp;
            pictureBoxCapture.Refresh();
        }

        private void connectionSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!connectionSettingsOkFlag)
            {
                connectionSettingsForm = new ConnectionSettingsForm(this);
                connectionSettingsForm.ShowDialog();
            }
            else
            {
                connectionSettingsForm = new ConnectionSettingsForm(this, username, ipAddress.ToString(), password, port.ToString());
                connectionSettingsForm.ShowDialog();
            }
        }
        private void connectButton_Click(object sender, EventArgs e)
        {
            // CONNESSIONE
            if (!connected)
            {
                if (first && !connectionSettingsOkFlag)
                {
                    connectionSettingsForm = new ConnectionSettingsForm(this);
                    connectionSettingsForm.ShowDialog();

                    // se si e' premuto Annulla...
                    if (!connectionSettingsOkFlag)
                        return;
                }

                initializeConnection();
            }
            // DISCONNESSIONE
            else
            {
                closeConnection("Disconnected by user.");
            }

        }

        #endregion

        #region GESTIONE CHAT

        private void receiveChatMessage()
        {
            streamReceiver = new StreamReader(tcpServer.GetStream());
            string response = "";
            try
            {
                response = streamReceiver.ReadLine();
            }
            catch (IOException)
            {
                // Server disconnesso. Oppure Client in disconnessione...
                this.Invoke(new UpdateLogCallback(this.updateLog), new object[] { "Problems connecting to Server. Try again." });
                return;
            }

            // PRIMO MESSAGGIO IN ASSOLUTO:
            // se il primo carattere ricevuto e' "1" la connessione e' OK
            if (response[0] == '1')
            {
                this.Invoke(new UpdateLogCallback(this.updateLog), new object[] { "Connected to Server" });

                IDataObject data = Clipboard.GetDataObject();
                if (data != null && data.GetDataPresent(DataFormats.Text))
                    buttonClipboard.Enabled = true;

                // leggiamo la porta per la condivisione schermo, mandataci dal Server
                port_capture = int.Parse(streamReceiver.ReadLine());
                // per ricevere la clipboard usiamo la porta ottenuta +1
                IPEndPoint ipEndPointClipboard = new IPEndPoint(ipAddress, port_capture +1);
                tcpClipboard = new TcpClient();
                tcpClipboard.Connect(ipEndPointClipboard);
                streamReceiverClipboard = new StreamReader(tcpClipboard.GetStream());
                // e deleghiamo il compito ad un altro thread
                Thread threadClipboard = new Thread(receiveClipboard);
                threadClipboard.SetApartmentState(ApartmentState.STA);
                threadClipboard.Start();

                this.Invoke(new UpdateLogCallback(this.updateLog), new object[] {
                    "Capture on port: " + port_capture +"\r\nClipboard on port: "+ (port_capture+1) });

                // apriamo la connessione per la cattura
                captureSocket = new CaptureSocket(ipAddress.ToString(), port_capture);
                // instanziamo il thread per la ricezione della cattura
                workerObject = new CaptureWorker(this, pictureBoxCapture, captureSocket);
                workerThread = new Thread(workerObject.DoWork);
                workerThread.Start();
                while (!workerThread.IsAlive) ; //aspettiamo finche' non e' operativo
            }
            // se non riceviamo "1" c'e' stato un errore
            else
            {
                string why = "Not connected: ";
                // il motivo e' contenuto nella risposta: "0|motivo", tolgo i primi 2 caratteri...
                why += response.Substring(2, response.Length - 2);
                try
                {
                    // aggiorniamo la form col motivo
                    this.Invoke(new CloseConnectionCallback(this.closeConnection), new object[] { why });
                }
                catch
                {
                    return;
                }
                return;
            }

            // LOOP di lettura messaggi
            while (connected)
            {
                // mostriamo sempre i messaggi nella textBox...
                try
                {
                    this.Invoke(new UpdateLogCallback(this.updateLog), new object[] { streamReceiver.ReadLine() });
                }
                catch
                {
                    string why = "No response from Server";

                    if (connected)
                    {
                        // chiudo connessione
                        this.Invoke(new CloseConnectionCallback(this.closeConnection), new object[] { why });
                        // disabilito gestione clipboard
                        this.Invoke(new DisableClipboardCallback(this.disableClipboard), new object[] { "Share Clipboard" });
                        // chiudo connessione clipboard
                        streamReceiverClipboard.Close();
                    }
                }
            }
        }

        private void sendChatMessage()
        {
            txtMessage.Text = txtMessage.Text.Replace("\r\n", " ");

            if (txtMessage.Lines.Length >0)
            {
                streamSender.WriteLine(txtMessage.Text);
                streamSender.Flush();
            }
            txtMessage.Text = txtMessage.Text.Replace("\r\n", "");
            txtMessage.Clear();
        }
        private void buttonSend_Click(object sender, EventArgs e)
        {
            sendChatMessage();
        }
        private void txtMessage_KeyUp(object sender, KeyEventArgs e)
        {
            // se ho premuto INVIO (ENTER)
            if (e.KeyData == Keys.Enter)
            {                
                sendChatMessage();
            }
        }
        private void updateLog(string strMessage)
        {
            // aggiornamento del log:
            // se e' un messaggio di Chat (contiene "USER says: MESSAGGE") coloro USER e MESSAGE

            txtLog.Font = new Font(txtLog.SelectionFont, FontStyle.Regular);
            string search = " says: ";
            int pos = 0;
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
        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            // scrollo sempre fino in fondo!
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
            txtLog.Refresh();
        }

        #endregion

        #region GESTIONE CLIPBOARD

        private void disableClipboard(string why)
        {
            buttonClipboard.Enabled = false;
            buttonClipboard.Text = why;
        }
        private void enableClipboard(string why)
        {
            buttonClipboard.Enabled = true;
            buttonClipboard.Text = why;
        }
        private void receiveClipboard()
        {
            byte[] buff = new byte[1024];
            string text, username;
            // path dei file ricevuti
            System.Collections.Specialized.StringCollection paths = new System.Collections.Specialized.StringCollection();

            //TcpClient clientStream = (TcpClient)stream;
            //NetworkStream networkStream = clientStream.GetStream();
            //StreamReader streamReader = new StreamReader(clientStream.GetStream());

            // I PRIMI 4 CARATTERI INDICANO IL TIPO DI DATO NELLA CLIPBOARD:
            // "File", "Text", "Imag"
            while (connected)
            {
                NetworkStream networkStream = tcpClipboard.GetStream();
                bool abort = false, error = false;

                try
                {
                    string msgLine = streamReceiverClipboard.ReadLine();

                    string type;
                    try { type = msgLine.Substring(0, 4); }
                    catch { continue; }

                    if (type.Equals("ACK."))
                    {
                        // il Server ha mandato ACK poiche' ha finito di ricevere la nostra clipboard
                        // -> riabilito tasto Clipboard

                        string reset = "Share Clipboard";
                        this.Invoke(new DisableClipboardCallback(enableClipboard), new object[] { reset });

                        continue;
                    }

                    if (!type.Equals("File") && !type.Equals("Text") && !type.Equals("Imag"))
                        continue;

                    username = msgLine.Substring(4);

                    string why = "Receiving...";
                    this.Invoke(new DisableClipboardCallback(this.disableClipboard), new object[] { why });
                    
                    #region FILE

                    string fileFullPath;

                    if (type.Equals("File"))
                    {
                        paths.Clear();

                        /* MESSAGGI TRASMISSIONE FILE:
                         * 1) "FileQ|U", dove Q e' la quantita' di file da ricevere, "|" e' un separatore dato che non si
                         *      conosce il numero di cifre di Q, e U e' lo username.
                         * 2a) "File: " per indicare l'imminente trasferimento
                         * 2b) in alternativa, "Abort"
                         * 3) se non si ha abortito, viene spedito il file serializzato
                         */

                        int separator_index = msgLine.IndexOf('|');
                        username = msgLine.Substring(separator_index + 1);
                        string quant_str = msgLine.Substring(4, separator_index - 4); //Substring(start, LENGTH!!!)

                        int quant = int.Parse(quant_str);

                        this.Invoke(new UpdateLogCallback(this.updateLog), new object[] {
                                "...Saving "+ username + "'s clipboard in: "+sharedFolderLocation });

                        for (int j = 0; j < quant; j++)
                        {
                            // per ogni file in ricezione, ottengo una stringa formata da "File: X" dove X e' il nome del file
                            // in caso di errore invece "Abort"
                            string line = streamReceiverClipboard.ReadLine();

                            if (line == null)
                            {
                                error = true;
                                break;
                            }
                            if (line.Equals("Abort"))
                            {
                                abort = true;
                                break;
                            }
                            if (!line.Substring(0, 6).Equals("File: "))
                            {
                                error = true;
                                break;
                            }

                            // trasferimento file!

                            string fileName = line.Substring(6);

                            byte[] clientData = Convert.FromBase64String(streamReceiverClipboard.ReadLine());
                            // apro file da scrivere
                            fileFullPath = Path.GetFullPath(@sharedFolderLocation + "\\")+ username +"_"+ fileName;
                            BinaryWriter bWrite = new BinaryWriter(File.Open(fileFullPath, FileMode.Create));

                            int offset = fileName.Length + 4;
                            // scrivo (con offset: "File: " (4?) + lunghezza nome file)
                            bWrite.Write(clientData, offset, clientData.Length - offset);
                            // chiudo!!
                            bWrite.Close();
                            // aggiungo alla lista dei file in locale
                            paths.Add(fileFullPath);
                            // aggiorno log ogni volta che salvo un file...
                            this.Invoke(new UpdateLogCallback(this.updateLog), new object[] {
                                "File saved in: '"+ fileFullPath +"'\r\n(Use 'File -> Open folder' to see shared files)" });
                        }
                        if (error)
                            MessageBox.Show(username + " encountered an error while sharing his clipboard.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        if (abort)
                            MessageBox.Show(username + " aborted his clipboard sharing.", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        else
                        {
                            // aggiorno la clipboard con tutti i path dei file salvati in locale!
                            this.Invoke(new UpdateClipboardCallback(updateClipboard), new object[] { paths });
                        }
                    }
                    #endregion

                    #region TEXT
                    else if (type.Equals("Text"))
                    {
                        byte[] bytes = new byte[tcpClipboard.ReceiveBufferSize];

                        // bloccante finche' non viene letto un byte
                        networkStream.Read(bytes, 0, (int)tcpClipboard.ReceiveBufferSize);
                        string text_bytes = Encoding.ASCII.GetString(bytes);
                        text = trimToEnd(text_bytes);
                        // (dovrei mettere un terminatore per evitare la lettura di clipboard consecutive...)

                        // devo incapsularlo in un oggetto IDataObject per memorizzarlo nella clipboard!
                        IDataObject dataClipboard = new DataObject();
                        dataClipboard.SetData(text);
                        Clipboard.SetDataObject(dataClipboard, true);

                        this.Invoke(new UpdateLogCallback(this.updateLog), new object[] {
                                username + "'s clipboard saved (Text): '"+ text +"'"});
                    }
                    #endregion

                    #region IMAGE
                    else if (type.Equals("Imag"))
                    {
                        Stream imageStream = tcpClipboard.GetStream();
                        IFormatter formatter = new BinaryFormatter();
                        Bitmap bmp = (Bitmap)formatter.Deserialize(imageStream);

                        Clipboard.SetImage(bmp);

                        this.Invoke(new UpdateLogCallback(this.updateLog), new object[] {
                                username + "'s clipboard saved (Image)" });
                    }
                    #endregion

                    // mando ACK al Server quando ho finito di ricevere
                    
                    StreamWriter sw = new StreamWriter(tcpClipboard.GetStream());
                    sw.WriteLine("ACK.");
                    sw.Flush();
                    
                    string enable = "Share Clipboard";
                    this.Invoke(new DisableClipboardCallback(enableClipboard), new object[] { enable });
                }
                catch(Exception)
                {
                    if (connected)
                    {
                        // si arriva qui quando (SE) il Server si e' disconnesso mentre spediva...
                        string reset = "Share Clipboard";
                        this.Invoke(new DisableClipboardCallback(enableClipboard), new object[] { reset });
                    }
                    
                    // chiudo???
                    streamReceiverClipboard.Close();
                    tcpClipboard.Close();
                    break;
                }

            }
            
        }
        private void updateClipboard(System.Collections.Specialized.StringCollection paths)
        {
            Clipboard.SetFileDropList(paths);
        }

        private void sendClipboardFile(object data)
        {
            StreamWriter sw = new StreamWriter(tcpClipboard.GetStream());
            IDataObject fileData = (IDataObject)data;

            string why = "Sending Clipboard...";
            this.Invoke(new DisableClipboardCallback(this.disableClipboard), new object[] { why });

            // Vettore di filenames presenti dentro la clipboard
            object fromClipboard = fileData.GetData(DataFormats.FileDrop, true);
            int qta = 0;
            foreach (string sourceFileName in (Array)fromClipboard)
                qta++;

            // controlli PRIMA di spedire l'header "FileQ|U"
            foreach (string sourceFileName in (Array)fromClipboard)
            {
                if (Directory.Exists(sourceFileName) || Path.GetFileName(sourceFileName) == "")
                {
                    MessageBox.Show("Send failed: can't share a directory!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    why = "Share Clipboard";
                    this.Invoke(new DisableClipboardCallback(this.enableClipboard), new object[] { why });
                    return;
                }

                try
                {
                    FileInfo fleMembers = new FileInfo(sourceFileName);
                    float size = (float)(fleMembers.Length / 1024 / 1024); //MB!
                    if (size > 50)
                    {
                        MessageBox.Show("Send failed: file '" + sourceFileName + "' is too big (50 MB max)!",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        why = "Share Clipboard";
                        this.Invoke(new DisableClipboardCallback(this.enableClipboard), new object[] { why });
                        return;
                    }
                }
                catch
                {
                    why = "Share Clipboard";
                    this.Invoke(new DisableClipboardCallback(this.enableClipboard), new object[] { why });
                    return;
                }
            }

            // mando header...
            try
            {
                // FileQ|U
                sw.WriteLine("File" + qta.ToString() + "|" + username);
                sw.Flush();
            }
            catch
            {
                why = "Share Clipboard";
                this.Invoke(new DisableClipboardCallback(this.enableClipboard), new object[] { why });
                return;
            }

            // mando file(s)...
            foreach (string sourceFileName in (Array)fromClipboard)
            {
                try
                {
                    // Conversione del file in stringa
                    byte[] fileNameByte = Encoding.ASCII.GetBytes(Path.GetFileName(sourceFileName));
                    byte[] fileBytes = File.ReadAllBytes(sourceFileName);
                    byte[] clientData = new byte[4 + fileNameByte.Length + fileBytes.Length];
                    BitConverter.GetBytes(fileNameByte.Length).CopyTo(clientData, 0);
                    fileNameByte.CopyTo(clientData, 4);
                    fileBytes.CopyTo(clientData, 4 + fileNameByte.Length);

                    // Invio messaggio al server
                    sw.WriteLine("File: " + Path.GetFileName(sourceFileName));
                    sw.Flush();
                    sw.WriteLine(Convert.ToBase64String(clientData));
                    sw.Flush();
                }
                catch
                {
                    this.Invoke(new UpdateLogCallback(this.updateLog), new object[] { "Clipboard send ABORTED!\r\n\r\n" });
                    sw.WriteLine("Abort");
                    sw.Flush();
                    break; // esco dal ciclo dei files
                }
            }
            //sw.Close();

        }
        private void buttonClipboard_Click(object sender, EventArgs e)
        {
            string dataText;
            IDataObject data;
            try
            {
                data = Clipboard.GetDataObject();
            }
            catch (ExternalException)
            {
                // da' questa eccezione in caso di concorrenza con altri programmi

                buttonClipboard.Text = "Clipboard busy. Retry.";
                return;
            }

            StreamWriter sw = new StreamWriter(tcpClipboard.GetStream());

            // TEXT
            if (data.GetDataPresent(DataFormats.Text))
            {
                try
                {
                    buttonClipboard.Enabled = false;
                    buttonClipboard.Text = "Sending Clipboard...";

                    sw.WriteLine("Text" + username);
                    sw.Flush();
                    NetworkStream netstream = tcpClipboard.GetStream();
                    dataText = (string)data.GetData(DataFormats.Text);
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(dataText);
                    netstream.Write(sendBytes, 0, sendBytes.Length);

                }
                catch (Exception )
                {
                    MessageBox.Show("Error sending clipboard");
                    buttonClipboard.Enabled = true;
                    buttonClipboard.Text = "Share Clipboard";
                    return;
                }
            }

            // FILE
            else if (data.GetDataPresent(DataFormats.FileDrop, true))
            {
                ParameterizedThreadStart parameterizedThreadSendFile = new ParameterizedThreadStart(sendClipboardFile);
                Thread threadSendFile = new Thread(parameterizedThreadSendFile);
                threadSendFile.Start(data);
            }

            // IMAGE
            else if (Clipboard.ContainsImage())
            {
                buttonClipboard.Enabled = false;
                buttonClipboard.Text = "Sending Clipboard...";
                Bitmap bmp = (Bitmap)Clipboard.GetImage();
               
                try
                {
                    sw.WriteLine("Imag" + username);
                    sw.Flush();

                    IFormatter formatter = new BinaryFormatter();
   
                    NetworkStream networkStream = tcpClipboard.GetStream();
                    formatter.Serialize(networkStream, bmp);

                }
                catch (Exception)
                {
                    MessageBox.Show("Error sending clipboard");

                    buttonClipboard.Enabled = true;
                    buttonClipboard.Text = "Share Clipboard";
                    return;
                }

                // il Client deve attendere un "ACK" da parte del Server per riattivare il tasto Clipboard
                // -> guarda funzione receiveClipboard
            }
            else
            {
                MessageBox.Show("Send failed: clipboard is empty", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonClipboard.Enabled = false;
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message msg)
        {
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

        #endregion

        #region MENU & BOTTONS

        private void toggleVideo_Click(object sender, EventArgs e)
        {
            if (show_video)
            {
                pictureBoxCapture.Hide();
                labelVideoWarning.Visible = false;
                //pictureBoxCapture.BackColor = Color.Beige;

                int offset = txtLog.Left - pictureBoxCapture.Left - 20;
                buttonConnection.Left += offset;
                buttonToggleVideo.Left += offset;
                buttonToggleChat.Left += offset;

                txtLog.Height -= buttonConnection.Height + 5;
                txtLog.Top += buttonConnection.Height + 10;

                int previous_left = this.Left;
                previous_width = this.Width;
                this.Width -= pictureBoxCapture.Width + 20;
                this.Left = previous_left;

                buttonToggleChat.Enabled = false;
                show_video = false;
            }
            else
            {
                pictureBoxCapture.Show();
                labelVideoWarning.Visible = true;
                pictureBoxCapture.BackColor = Color.DimGray;

                int previous_left = this.Left;
                this.Width = previous_width;
                this.Left = previous_left;

                int offset = txtLog.Left - pictureBoxCapture.Left - 20;
                buttonConnection.Left -= offset;
                buttonToggleVideo.Left -= offset;
                buttonToggleChat.Left -= offset;

                txtLog.Height += buttonConnection.Height + 5;
                txtLog.Top -= buttonConnection.Height + 10;

                labelVideoWarningCenter();

                buttonToggleChat.Enabled = true;
                show_video = true;

            }
        }
        private void toggleChat_Click(object sender, EventArgs e)
        {
            if (show_chat)
            {
                txtLog.Hide();
                txtMessage.Hide();
                buttonSend.Hide();
                buttonClipboard.Hide();

                buttonToggleVideo.Enabled = false;
                show_chat = false;

                int free_space = txtLog.Width + 40;

                int previous_left = this.Left;
                previous_width = this.Width;
                this.Width -= free_space;
                this.Left = previous_left;

                pictureBoxCapture.Width += free_space;
                pictureBoxCapture.Refresh();
                buttonConnection.Left += free_space;
                buttonToggleChat.Left += free_space;
                buttonToggleVideo.Left += free_space;

                labelVideoWarningCenter();
            }
            else
            {
                txtLog.Show();
                txtMessage.Show();
                buttonSend.Show();
                buttonClipboard.Show();

                int free_space = txtLog.Width + 40;

                pictureBoxCapture.Width -= free_space;
                pictureBoxCapture.Refresh();
                buttonConnection.Left -= free_space;
                buttonToggleChat.Left -= free_space;
                buttonToggleVideo.Left -= free_space;

                int previous_left = this.Left;
                this.Width = previous_width;
                this.Left = previous_left;

                labelVideoWarningCenter();

                buttonToggleVideo.Enabled = true;
                show_chat = true;
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                streamSender.WriteLine(disconnectionMessage);
                closeConnection("Exiting");

                //connected = false;
                //streamSender.Close();
                //streamReceiver.Close();
                //streamReceiverClipboard.Close();
                //tcpServer.Close();
            }
            this.Close();
        }
        private void helpQuestionMarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(helpString, "Help", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dirPath = @sharedFolderLocation;
            string windir = Environment.GetEnvironmentVariable("WINDIR");

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = windir + @"\explorer.exe";
            process.StartInfo.Arguments = dirPath;
            process.Start();
        }
       
        #endregion

        private string trimToEnd(string input)
        {
            int index = input.IndexOf('\0');
            if (index < 0)
                return input;

            return input.Substring(0, index);
        }
    }
}
