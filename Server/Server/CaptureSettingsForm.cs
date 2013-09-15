using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public partial class CaptureSettingsForm : Form
    {
        /* CAPTURE TYPE: 1 = PARTE DELLO SCHERMO, 2 = FINESTRA ATTIVA, 3 = SCHERMO INTERO */
        private int captureType;

        private MainForm mainForm;
        private bool init;
        private bool disable;

        private string description_1 = "Captures the specified area of the screen.";
        private string description_2 = "Captures the area of the screen corresponding to the active window (Default).";
        private string description_3 = "Captures the entire screen.";

        private int x_s, y_s, w_s, h_s;
        private int scaleWidth = Screen.PrimaryScreen.Bounds.Size.Width / 100;
        private int scaleHeight = Screen.PrimaryScreen.Bounds.Size.Height / 100;

        public void setCaptureType(int type)
        {
            captureType = type;
        }
        public int getCaptureType()
        {
            return captureType;
        }
        public void setCaptureScreenMeasures(int x, int y, int w, int h)
        {
            x_s = x;
            y_s = y;
            w_s = w;
            h_s = h;
        }

        public CaptureSettingsForm(MainForm mf)
        {
            this.mainForm = mf;

            // se e' in corso una cattura devo disabilitare i campi della forma!
            disable = true;

            if (!MainForm.isCapturing())
                disable = false;
            else
                if (!mf.connected)
                    disable = false;

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                setCaptureType(1);

            if (radioButton2.Checked)
            {
                try
                {
                    setCaptureScreenMeasures(int.Parse(textBox1.Text), int.Parse(textBox2.Text), int.Parse(textBox3.Text), int.Parse(textBox4.Text));
                }
                catch
                {
                    MessageBox.Show("Some parameters are incorrect, check if values are all positive numbers", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                setCaptureType(2);
                mainForm.setCaptureScreenMeasures(x_s, y_s, w_s, h_s);
            }

            if (radioButton3.Checked)
                setCaptureType(3);
            
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CaptureSettingsForm_Load(object sender, EventArgs e)
        {
            /* init serve esclusivamente per delimitare il momento di apertura della finestra.
             * in particolare, lo usiamo per l'evento di cambiamento del radioButton
             * riguardante i tipi di cattura:
             * il selettore di area di schermo deve aprirsi quando clicchiamo sulla relativa opzione,
             * non quando apriamo la finestra.
             */
            init = true;

            this.Left = mainForm.Left;
            this.Top = mainForm.Top;
            richTextBox1.BackColor = this.BackColor;

            // carichiamo la form col valore gia' presente nell'applicazione...
            setCaptureType(mainForm.getCaptureType());
            Rectangle r = mainForm.getWindowRectangle();

            setCaptureScreenMeasures(r.X, r.Y, r.Width, r.Height);

            if (captureType == 1) { radioButton1.Checked = true; groupBox2.Enabled = false; }
            if (captureType == 2) { radioButton2.Checked = true; groupBox2.Enabled = true; }
            if (captureType == 3) { radioButton3.Checked = true; groupBox2.Enabled = false; }

            toolTip1.SetToolTip(radioButton1, description_2);
            toolTip1.SetToolTip(radioButton2, description_1);
            toolTip1.SetToolTip(radioButton3, description_3);

            init = false;

            if (disable)
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
            }

            this.AutoSize = true;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;


        }

        private void updateMeasuresTextboxes()
        {
            textBox1.Text = x_s.ToString();
            textBox2.Text = y_s.ToString();
            textBox3.Text = w_s.ToString();
            textBox4.Text = h_s.ToString();
        }

        private void updateMeasuresTextboxes(string s1, string s2, string s3, string s4)
        {
            textBox1.Text = s1;
            textBox2.Text = s2;
            textBox3.Text = s3;
            textBox4.Text = s4;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            // FINESTRA

            richTextBox1.Text = description_2;
            groupBox2.Enabled = false;
            if (radioButton1.Checked)
                updateMeasuresTextboxes("", "", "", "");
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            // PARTE DI SCHERMO

            richTextBox1.Text = description_1;
            groupBox2.Enabled = true;
            if ((radioButton2.Checked) && (!init))
                selectCaptureArea();

            updateMeasuresTextboxes();


        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            // SCHERMO INTERO

            richTextBox1.Text = description_3;
            groupBox2.Enabled = false;

            if (radioButton3.Checked)
                updateMeasuresTextboxes(Screen.PrimaryScreen.Bounds.X.ToString(),
                    Screen.PrimaryScreen.Bounds.Y.ToString(),
                    Screen.PrimaryScreen.Bounds.Size.Width.ToString(),
                    Screen.PrimaryScreen.Bounds.Size.Height.ToString());
        }

        private void selectCaptureArea()
        {
            Rectangle rect;
            ScreenRegionSelector selector = new ScreenRegionSelector();
            selector.ShowDialog();
            rect = selector.getRectangle();
            setCaptureScreenMeasures(rect.X, rect.Y, rect.Width, rect.Height);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // default measures...
            setCaptureScreenMeasures(200, 200, 200, 200);
            updateMeasuresTextboxes();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // new area selection

            selectCaptureArea();
            updateMeasuresTextboxes();
        }

    }
}