using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace Server
{
    public partial class ConnectionSettingsForm : Form
    {
        public MainForm mainForm;

        public ConnectionSettingsForm(MainForm mf, bool flg)
        {
            InitializeComponent();
            setIpAddress();
            txtPort.Text = "5566";
            mainForm = mf;
            //OpenFromOpzioni = flg;
        }
        // apertura con parametri gia' settati
        public ConnectionSettingsForm(MainForm m, int address, string psw, string port, bool flg)
        {
            InitializeComponent();
            setIpAddress();
            mainForm = m;
            txtPassword.Text = psw;
            txtPort.Text = port;
            txtIp.SelectedIndex = address;
            //OpenFromOpzioni = flg;
        }

        private void ConnectionSettingsForm_Load(object sender, EventArgs e)
        {
            this.Left = mainForm.Left;
            this.Top = mainForm.Top;

            this.AutoSize = true;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            if (mainForm.setUsername(txtUsername.Text) && mainForm.setPassword(txtPassword.Text)
                && mainForm.setPort(txtPort.Text))
            {
                if (txtIp.SelectedIndex == -1)
                    MessageBox.Show("IP address is not valid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    mainForm.setIp(txtIp.SelectedItem.ToString());
                    mainForm.setIpDropdownMenuIndex(txtIp.SelectedIndex);
                    // setto =1 il flag della mainForm per informarla che la configurazione della connessione e' andata a buon fine
                    mainForm.setConnectionSettingsOkFlag(true);
                    this.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // metodo per riempire la dropdownlist con tutti gli IP della mia scheda di rete
        private void setIpAddress()
        {
            string myHost = System.Net.Dns.GetHostName();
            System.Net.IPHostEntry myIPs = System.Net.Dns.GetHostEntry(myHost);

            foreach (System.Net.IPAddress myIP in myIPs.AddressList)
                if (myIP.AddressFamily.ToString() == System.Net.Sockets.ProtocolFamily.InterNetwork.ToString())
                    txtIp.Items.Add(myIP.ToString());

            txtIp.DropDownStyle = ComboBoxStyle.DropDownList;
            txtIp.SelectedIndex = (myIPs.AddressList.Length == 0) ? -1 : 0;
        }

    }
}