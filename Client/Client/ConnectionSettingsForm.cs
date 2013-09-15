using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace Client
{
    public partial class ConnectionSettingsForm : Form
    {
        public MainForm mainForm;

        public ConnectionSettingsForm()
        {
            InitializeComponent();
        }
        public ConnectionSettingsForm(MainForm mf, string username, string ipAddr, string password, string port)
        {
            InitializeComponent();
            this.mainForm = mf;

            txtUsername.Text = username;
            txtIp.Text = ipAddr;
            txtPassword.Text = password;
            txtPort.Text = port;
        }
        public ConnectionSettingsForm(MainForm form)
        {
            InitializeComponent();
            this.mainForm = form;

            // come testo IP di default scelgo il primo indirizzo di rete (utile per debug in localhost...)
            string myHost = System.Net.Dns.GetHostName();
            System.Net.IPHostEntry myIPs = System.Net.Dns.GetHostEntry(myHost);
            //txtIp.Text = myIPs.AddressList[0].ToString();
            foreach (System.Net.IPAddress myIP in myIPs.AddressList)
                if (myIP.AddressFamily.ToString() == System.Net.Sockets.ProtocolFamily.InterNetwork.ToString())
                {
                    txtIp.Text = myIP.ToString();
                    break;
                }
            txtPort.Text = "5566";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (mainForm.setUsername(txtUsername.Text) && mainForm.setPassword(txtPassword.Text)
                &&  mainForm.setIp(txtIp.Text) && mainForm.setPort(txtPort.Text))
            {
                    mainForm.setConnectionSettingsOkFlag();
                    this.Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}