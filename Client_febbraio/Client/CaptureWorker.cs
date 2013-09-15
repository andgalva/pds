using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;

namespace Client
{
    public class CaptureWorker
    {
        private PictureBox pictureBox;
        private MainForm mainForm;
        private CaptureSocket captureSocket;
        private Bitmap bmp;
        private int result;

        private volatile bool stop = false;

        public CaptureWorker(MainForm mf, PictureBox pb, CaptureSocket cs)
        {
            this.mainForm = mf;
            this.captureSocket = cs;
            this.pictureBox = pb;
        }

        public void DoWork()
        {
            while (!stop)
            {
                try
                {
                    result = captureSocket.receiveCapture(ref bmp);
                    if (result == -1) // socket non connessa
                        continue;
                    
                    mainForm.Invoke(new updateImage(mainForm.updateImage), bmp, (result==1));
                    
                    Thread.Sleep(30);
                }
                catch { }
            }

            // se stop == true
            captureSocket.closeCaptureConnection();
        }

        public void forceStop()
        {
            stop = true;
        }

        public bool isStopped()
        {
            return stop;
        }
    }
}


